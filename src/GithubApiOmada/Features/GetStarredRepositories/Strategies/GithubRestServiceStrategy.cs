using GithubApiOmada.Features.GetStarredRepositories;
using Microsoft.Net.Http.Headers;

namespace GithubApiOmada.Features.GetStarredRepositories.Strategies
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class GithubRestServiceStrategy : GetRepositoriesStrategy
    {
        private readonly IHttpClientFactory _clientFactory;

        public Strategy Strategy { get; } = Strategy.GithubRest;

        public GithubRestServiceStrategy(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<List<GetGithubRepositories.Response>> GetRepositories(string githubPersonalToken, CancellationToken cancellationToken)
        {
            var client = GetNewClient(githubPersonalToken);            
            var repositories = await client.GetFromJsonAsync<List<GetGithubRepositories.Response>>(GetGithubRepositories.GithubRoute, cancellationToken);
            return repositories ?? new();
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
    }
}
