using Ardalis.ApiEndpoints;
using GithubApiOmada.Features.GetSimilarRepositories;
using GithubApiOmada.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GithubApiOmada.Features.GetRepositories
{
    public class GetStarredRepositoriesEndpoint : BaseAsyncEndpoint
                                                    .WithRequest<GetGithubRepositories.Request>
                                                    .WithResponse<IEnumerable<GetGithubRepositories.Response>>
    {
        private const string GPL_LICENSE = "gpl";
        private const string SIMILAR_REPO_LINK = "Similar repositories";

        private readonly GithubDbContext _database;
        private readonly IEnumerable<GetRepositoriesStrategy> _strategies;

        public GetStarredRepositoriesEndpoint(GithubDbContext database, IEnumerable<GetRepositoriesStrategy> strategies)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
        }

        [HttpGet(GetGithubRepositories.RouteTemplate)]
        [SwaggerOperation(
            Summary = "Gets starred repository for user",
            Description = "Get all starred repos for the user represented by personal token",
            OperationId = "GetStarredRepositoriesEndpoint",
            Tags = new[] { "Starred-Repositories" })
        ]
        public override async Task<ActionResult<IEnumerable<GetGithubRepositories.Response>>> HandleAsync(
            [FromQuery] GetGithubRepositories.Request request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                List<GetGithubRepositories.Response>? repositories = await GetRepositories(request.forceRestRead, request.Token, cancellationToken);
                return Ok(repositories);
            }
            catch
            {
                return BadRequest("Error fetching github Api");
            }
        }

        /// <summary>
        /// Get repositories from database or github depending on force parameter
        /// </summary>
        /// <param name="force">Force to read databa from rest service</param>
        /// <param name="githubPersonalToken">Token for github rest service</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<List<GetGithubRepositories.Response>> GetRepositories(bool force, string githubPersonalToken, CancellationToken cancellationToken)
        {
            List<GetGithubRepositories.Response>? repositories = new();

            if (force)
            {
                GetRepositoriesStrategy strategy = _strategies.Single(c => c.Strategy == Strategy.GithubRest);
                repositories = await strategy.GetRepositories(githubPersonalToken, cancellationToken);
                await SaveRepositoriesToDatabase(repositories, cancellationToken);
            }

            else
            {
                GetRepositoriesStrategy strategy = _strategies.Single(c => c.Strategy == Strategy.Database);
                repositories = await strategy.GetRepositories(githubPersonalToken, cancellationToken);
            }

            //Add link to the similarities endpoint to gpl starred repositories
            repositories?
                .Where(c => c.license?.key?.Contains(GPL_LICENSE, StringComparison.CurrentCultureIgnoreCase) ?? false)
                .ToList()
                .ForEach(c => c.Urls.Add(SIMILAR_REPO_LINK, Url.Link(nameof(GetSimilarRepositoriesEndpoint), new { RepositoryName = c.name })));
            return repositories ?? new();
        }

        /// <summary>
        /// Save repositories to database
        /// </summary>
        /// <param name="response">Repositories read from github api</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SaveRepositoriesToDatabase(List<GetGithubRepositories.Response> response, CancellationToken cancellationToken)
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
    }
}