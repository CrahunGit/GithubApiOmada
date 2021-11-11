using Ardalis.ApiEndpoints;
using GithubApiOmada.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace GithubApiOmada.Features.GetRepositories
{
    public class GetStarredRepositoriesEndpoint : BaseAsyncEndpoint
                                                    .WithRequest<GetGithubRepository.Request>
                                                    .WithResponse<IEnumerable<GetGithubRepository.Response>>
    {
        private const string GPL_LICENSE = "gpl";
        private const string SIMILAR_REPO_LINK = "Similar repositories";

        private readonly IHttpClientFactory _clientFactory;
        private readonly GithubDbContext _database;

        public GetStarredRepositoriesEndpoint(IHttpClientFactory clientFactory, GithubDbContext database)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        [HttpGet(GetGithubRepository.RouteTemplate)]
        [SwaggerOperation(
            Summary = "Gets starred repository for user",
            Description = "Get all starred repos for the user represented by personal token",
            OperationId = "Author.Repositories.GetList",
            Tags = new[] { "Starred-Repositories" })
        ]
        public override async Task<ActionResult<IEnumerable<GetGithubRepository.Response>>> HandleAsync(
            [FromQuery] GetGithubRepository.Request request,
            CancellationToken cancellationToken = default)
        {
            HttpClient client = GetNewClient(request.token);

            try
            {
                List<GetGithubRepository.Response>? repositories = await GetRepositories(request.forceRestRead, client, cancellationToken);
                return Ok(repositories);
            }
            catch
            {
                return BadRequest("Error fetching github Api");
            }
        }

        /// <summary>
        /// Get new configured httpclient
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HttpClient GetNewClient(string token)
        {
            HttpClient? client = _clientFactory.CreateClient("github");
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"token {token}");
            return client;
        }

        /// <summary>
        /// Get repositories from github rest api
        /// </summary>
        /// <param name="force">Force to read databa from rest service</param>
        /// <param name="client">Configured httpclient for the request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<List<GetGithubRepository.Response>> GetRepositories(bool force, HttpClient client, CancellationToken cancellationToken)
        {
            List<GetGithubRepository.Response>? repositories = new();

            if (force is false) 
            {
                repositories = await GetRepositoriesFromDatabase(cancellationToken);
            }

            if (repositories.Any() is false)
            {
                repositories = await client.GetFromJsonAsync<List<GetGithubRepository.Response>>(GetGithubRepository.GithubRoute, cancellationToken);
                await SaveRepositoriesToDatabase(repositories, cancellationToken);
            }

            //Add link to the similarities endpoint to gpl starred repositories
            repositories?
                .Where(c => c.license?.key?.Contains(GPL_LICENSE, StringComparison.CurrentCultureIgnoreCase) ?? false)
                .ToList()
                .ForEach(c => c.urls.Add(new(SIMILAR_REPO_LINK, url: Url.Link(nameof(GetSimilarRepositories), null))));
            return repositories;
        }

        /// <summary>
        /// Save repositories response to database
        /// </summary>
        /// <param name="response">Read repositories from github api</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SaveRepositoriesToDatabase(List<GetGithubRepository.Response> response, CancellationToken cancellationToken)
        {
            _database.Repositories.RemoveRange(_database.Repositories);
            _database.Repositories.AddRange(response.Select(r => new GithubRepository
            {
                Name = r.name,
                License = r.license is not null ? new License
                {
                    Key = r.license.key,
                    Name = r.license.name,
                } : null
            }));

            await _database.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Get repositories response from database
        /// </summary>
        /// <param name="response">Read repositories from github api</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<List<GetGithubRepository.Response>> GetRepositoriesFromDatabase(CancellationToken cancellationToken)
        {
            return await _database.Repositories.Select(m => new GetGithubRepository.Response(
                m.Id,
                m.Name,
                m.License != null ? new GetGithubRepository.License(m.License.Key, m.License.Name) : null)
            ).ToListAsync(cancellationToken);
        }
    }
}