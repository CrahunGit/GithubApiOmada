using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
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
            Summary = "Find similar repository that is not under GPL license",
            Description = "Get similar repositories that are not under GPL license",
            OperationId = "GetSimilarRepositories",
            Tags = new[] { "Similar-Repositories" })
        ]
        public override async Task<ActionResult<IEnumerable<GetSimilarRepositories.Response>>> HandleAsync(
            [FromQuery] GetSimilarRepositories.Request request,
            CancellationToken cancellationToken = default)
        {
            HttpClient? client = _clientFactory.CreateClient("github");

            try
            {
                GetSimilarRepositories.Response? repositories = await client.GetFromJsonAsync<GetSimilarRepositories.Response>(string.Format(GetSimilarRepositories.GithubRoute, request.repositoryName), cancellationToken);
                return Ok(repositories?.repositories);
            }
            catch
            {
                return BadRequest("Error fetching github Api");
            }
        }
    }
}