﻿using Microsoft.AspNetCore.Mvc;

namespace GithubApiOmada.Features.GetRepositories
{
    public record GetGithubRepositories(int id, string name, GetGithubRepositories.License license)
    {
        public const string RouteTemplate = "/api/starred-repositories";
        public const string GithubRoute = "/user/starred";

        public record License(string key, string name);

        public record Response(int id, string name, License license)
        {
            public Dictionary<string, string?> Urls { get; set; } = new();
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