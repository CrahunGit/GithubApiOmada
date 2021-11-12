using FluentAssertions;
using GithubApiOmada.Features.GetStarredRepositories;
using GithubApiOmada.Infrastructure.Persistence;
using IntegrationTests.Infrastructure.Configuration;
using IntegrationTests.Infrastructure.Fixtures;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Tests.Documents
{
    [Collection(Constants.Api)]
    [AutoRollback]
    public class GetStarredRepositoriesEndpointTest
    {
        private const string API_PATH = "/api/starred-repositories";
        private readonly TestHostFixture fixture;

        public GetStarredRepositoriesEndpointTest(TestHostFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetStarredUserRepositories_EmptyDatabase_GetsNoItems()
        {
            //Arrange
            HttpClient client = fixture.Server.CreateClient();
            client.DefaultRequestHeaders.Add("token", "any_token");

            //Act
            IEnumerable<GetGithubRepositories.Response> response = await client.GetFromJsonAsync<IEnumerable<GetGithubRepositories.Response>>($"{API_PATH}");

            //Assert
            response.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetStarredUserRepositories_Database1Repository_Gets1Repository()
        {
            //Arrange
            SeedDatabase();
            HttpClient client = fixture.Server.CreateClient();
            client.DefaultRequestHeaders.Add("token", "any_token");

            //Act
            IEnumerable<GetGithubRepositories.Response> response = await client.GetFromJsonAsync<IEnumerable<GetGithubRepositories.Response>>($"{API_PATH}");

            //Assert
            response.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetStarredUserRepositories_ForceRestRead_GetsBadRequestFromGithub()
        {
            //Arrange
            SeedDatabase();
            HttpClient client = fixture.Server.CreateClient();
            client.DefaultRequestHeaders.Add("token", "any_token");

            //Act
            HttpResponseMessage response = await client.GetAsync($"{API_PATH}?forceRestRead=true");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetStarredUserRepositories_NoToken_GetsBadRequest()
        {
            //Arrange
            SeedDatabase();
            HttpClient client = fixture.Server.CreateClient();

            //Act
            HttpResponseMessage response = await client.GetAsync($"{API_PATH}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Seeds database with test values
        /// </summary>
        /// <returns></returns>
        private void SeedDatabase()
        {
            fixture.Server.Host.SeedDbContext<GithubDbContext>((ctx, sp) =>
            {
                ctx.Add(new GithubRepository { Name = "test", License = new License { Key = "gpl", Name = "gpl" } });
                ctx.SaveChanges();
            });
        }
    }
}
