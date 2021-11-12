using Microsoft.AspNetCore.Mvc;

namespace GithubApiOmada.Features.GetSimilarRepositories
{
    public record GetSimilarRepositories(int id, string name, GetSimilarRepositories.License license)  
    {
        public const string RouteTemplate = "/api/similar-repositories";
        public const string GithubRoute = "/search/repositories?q={0}&sort=stars&order=desc";

        public record License(string key, string name);

        public record Response(int id, string name)
        {
            public Dictionary<string, string?> Urls { get; set; } = new();
        }

        public record Owner(int id, string login);

        public class Request
        {
            [FromHeader]
            public string? Token { get; set; }

            [FromQuery]
            public string? RepositoryName { get; set; }
        }
    }
}
