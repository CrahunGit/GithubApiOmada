using FluentAssertions;
using GithubApiOmada.Features.GetSimilarRepositories;
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
    public class GetSimilarRepositoriesEndpointTest
    {
        private const string API_PATH = "/api/similar-repositories?";
        private readonly TestHostFixture fixture;

        public GetSimilarRepositoriesEndpointTest(TestHostFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetStarredUserRepositories_AnySearch_GetsFewItemsFromGithub()
        {
            //Arrange
            HttpClient client = fixture.Server.CreateClient();

            //Act
            IEnumerable<GetSimilarRepositories.Response> response = await client.GetFromJsonAsync<IEnumerable<GetSimilarRepositories.Response>>($"{API_PATH}&repositoryName=tetris");

            //Assert
            response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetStarredUserRepositories_AnyBadSearch_GetsNoItemsFromGithub()
        {
            //Arrange
            HttpClient client = fixture.Server.CreateClient();

            //Act
            IEnumerable<GetSimilarRepositories.Response> response = await client.GetFromJsonAsync<IEnumerable<GetSimilarRepositories.Response>>($"{API_PATH}&repositoryName=netdata");

            //Assert
            response.Should().HaveCount(0);
        }
    }
}
