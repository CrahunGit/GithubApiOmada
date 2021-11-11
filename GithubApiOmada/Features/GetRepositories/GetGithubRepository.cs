using Microsoft.AspNetCore.Mvc;

namespace GithubApiOmada.Features.GetRepositories
{
    public record GetGithubRepository(int id, string name, GetGithubRepository.License license)
    {
        public const string RouteTemplate = "/api/starred-repositories";
        public const string GithubRoute = "/user/starred";

        public record License(string key, string name);

        public record Response(int id, string name, License license)
        {
            public List<Url> urls { get; set; } = new List<Url>();

            public record Url(string key, string? url);
        }

        public class Request
        {
            [FromHeader]
            public string Token { get; set; }

            [FromQuery]
            public bool forceRestRead { get; set; }
        }
    }
}