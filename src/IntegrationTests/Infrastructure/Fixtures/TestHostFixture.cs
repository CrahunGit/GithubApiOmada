#region "Namespace imports"

using GithubApiOmada.Infrastructure.Persistence;
using IntegrationTests.Infrastructure.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

#endregion

namespace IntegrationTests.Infrastructure.Fixtures
{
    /// <summary>
    ///     Fixture to configure and launch the test host. Reset database after each test
    ///     and remove database when all tests finished.
    ///     It will also add the new appsettings for tests
    /// </summary>
    public class TestHostFixture : IDisposable, IAsyncLifetime
    {
        #region "Fields"

        private static IConfigurationRoot configuration;

        public TestServer Server;

        #endregion

        #region "Method overrides"

        public Task InitializeAsync()
        {
            configuration = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.Tests.json")
                           .Build();

            IWebHostBuilder host = new WebHostBuilder()
                                  .UseContentRoot(Directory.GetCurrentDirectory())
                                  .UseConfiguration(configuration)
                                  .UseTestServer()
                                  .UseStartup<TestStartup>();

            Server = new TestServer(host)
            {
                PreserveExecutionContext = true
            };

            GithubDbContext context = Server.Services.GetService<GithubDbContext>();
            context?.Database.EnsureDeleted();
            context?.Database.EnsureCreated();

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Server.Dispose();
        }

        #endregion
    }
}