using Ardalis.ApiEndpoints;
using GithubApiOmada.Features.GetRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace GithubApiOmada.Features.GetSimilarRepositories
{
    public class GetSimilarRepositories : BaseAsyncEndpoint
                                                    .WithRequest<string>
                                                    .WithResponse<IEnumerable<GetGithubRepository>>
    {
        private readonly IHttpClientFactory _clientFactory;

        public GetSimilarRepositories(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        [HttpGet("/api/similar-repositories", Name = nameof(GetSimilarRepositories))]
        [SwaggerOperation(
            Summary = "Gets starred repository for user",
            Description = "Get all starred repos for the user represented by personal token",
            OperationId = "Author.Repositories.GetList",
            Tags = new[] { "Similar-Repositories" })
        ]
        public override async Task<ActionResult<IEnumerable<GetGithubRepository>>> HandleAsync(
            [FromHeader(Name = "token")] string token,
            CancellationToken cancellationToken = default)
        {
            var client = _clientFactory.CreateClient("github");
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"token {token}");

            try
            {
                var repositories = await client.GetFromJsonAsync<IEnumerable<GetGithubRepository>>("/user/starred", cancellationToken);
                return Ok(repositories);
            }
            catch
            {
                return BadRequest("Error fetching github Api");
            }
        }
    }
}