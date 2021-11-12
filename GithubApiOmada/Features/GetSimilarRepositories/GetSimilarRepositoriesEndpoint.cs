using Ardalis.ApiEndpoints;
using GithubApiOmada.Features.GetRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace GithubApiOmada.Features.GetSimilarRepositories
{
    public class GetSimilarRepositoriesEndpoint : BaseAsyncEndpoint
                                                    .WithRequest<GetSimilarRepositories.Request>
                                                    .WithResponse<IEnumerable<GetSimilarRepositories.Response>>
    {
        private readonly IHttpClientFactory _clientFactory;

        public GetSimilarRepositoriesEndpoint(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        [HttpGet(GetSimilarRepositories.RouteTemplate, Name = nameof(GetSimilarRepositoriesEndpoint))]
        [SwaggerOperation(
            Summary = "Gets starred repository for user",
            Description = "Get similar repositories that are not under GPL license",
            OperationId = "GetSimilarRepositories",
            Tags = new[] { "Similar-Repositories" })
        ]
        public override async Task<ActionResult<IEnumerable<GetSimilarRepositories.Response>>> HandleAsync(
            [FromQuery] GetSimilarRepositories.Request request,
            CancellationToken cancellationToken = default)
        {
            var client = _clientFactory.CreateClient("github");

            try
            {
                var repositories = await client.GetFromJsonAsync<GetSimilarRepositories.Response>(string.Format(GetSimilarRepositories.GithubRoute, request.RepositoryName), cancellationToken);
                return Ok(repositories?.repositories);
            }
            catch
            {
                return BadRequest("Error fetching github Api");
            }
        }
    }
}