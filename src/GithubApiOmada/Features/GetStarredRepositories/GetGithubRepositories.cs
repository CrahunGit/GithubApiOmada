using Microsoft.AspNetCore.Mvc;

namespace GithubApiOmada.Features.GetStarredRepositories
{
    public record GetGithubRepositories
    {
        public const string RouteTemplate = "/api/starred-repositories";
        public const string GithubRoute = "/user/starred";

        public record License(string key, string name);

        public record Response(int id, string name, License license)
        {
            public Dictionary<string, string?> urls { get; set; } = new();
        }

        public class Request
        {
            [FromHeader]
            public string token { get; set; }

            [FromQuery]
            public bool forceRestRead { get; set; }
        }
    }
}