using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GithubApiOmada.Features.GetSimilarRepositories
{
    public record GetSimilarRepositories
    {
        public const string RouteTemplate = "/api/similar-repositories";
        public const string GithubRoute = "/search/repositories?q={0}%3Agpl%3Ain%3Alicense&sort=stars&order=desc";

        public record Response
        {
            [JsonPropertyName("items")]
            public Repository[]? repositories { get; set; }
        }

        public record Repository(int id, string name, string description)
        {
            [JsonPropertyName("html_url")]
            public string? url { get; set; }
        }

        public class Request
        {
            [FromQuery]
            public string? RepositoryName { get; set; }
        }
    }
}
