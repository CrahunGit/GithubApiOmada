using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace GithubApiOmada.Features.GetRepositories
{
    public class GetStarredRepositoriesEndpoint : BaseAsyncEndpoint
                                                    .WithRequest<string>
                                                    .WithResponse<IEnumerable<GetGithubRepository.Response>>
    {
        const string GPL_LICENSE = "gpl";
        const string SIMILAR_REPO_LINK = "Similar repositories";

        private readonly IHttpClientFactory _clientFactory;

        public GetStarredRepositoriesEndpoint(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        [HttpGet(GetGithubRepository.RouteTemplate)]
        [SwaggerOperation(
            Summary = "Gets starred repository for user",
            Description = "Get all starred repos for the user represented by personal token",
            OperationId = "Author.Repositories.GetList",
            Tags = new[] { "Starred-Repositories" })
        ]
        public override async Task<ActionResult<IEnumerable<GetGithubRepository.Response>>> HandleAsync(
            [FromHeader(Name = "token")] string token,
            CancellationToken cancellationToken = default)
        {
            var client = _clientFactory.CreateClient("github");
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"token {token}");

            try
            {
                var repositories = await client.GetFromJsonAsync<List<GetGithubRepository.Response>>(GetGithubRepository.GithubRoute, cancellationToken);
                
                //Add link to the similarities endpoint to gpl starred repositories
                repositories?
                    .Where(c => c.license?.key?.Contains(GPL_LICENSE, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    .ToList()
                    .ForEach(c => c.urls.Add(new (SIMILAR_REPO_LINK, url: Url.Link(nameof(GetSimilarRepositories), null))));

                return Ok(repositories);
            }
            catch
            {
                return BadRequest("Error fetching github Api");
            }
        }
    }
}