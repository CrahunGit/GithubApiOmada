#region "Namespace imports"

using GithubApiOmada.Features.GetStarredRepositories;
using GithubApiOmada.Features.GetStarredRepositories.Strategies;
using GithubApiOmada.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json.Serialization;

#endregion

namespace IntegrationTests.Infrastructure.Configuration
{
    /// <summary>
    ///     Startup class that will be launched  with the test host instead of the original startup class
    ///     from Host project
    /// </summary>
    public class TestStartup
    {
        #region "Fields"

        private readonly IConfiguration configuration;

        #endregion

        #region "Constructors"

        public TestStartup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region "Public methods"

        /// <summary>
        ///     Replace the needed dependencies with test dependencies
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(j =>
                {
                    j.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    j.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Add services to the container.
            services.AddHttpClient("github", c =>
            {
                c.BaseAddress = new Uri("https://api.github.com/");
                c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                c.DefaultRequestHeaders.Add("User-Agent", "OmadaSample");
            });

            services.AddDbContext<GithubDbContext>(opt => opt.UseSqlServer(services.BuildServiceProvider().GetService<IConfiguration>().GetConnectionString("Default")));
            services.AddScoped<GetRepositoriesStrategy, GithubRestServiceStrategy>();
            services.AddScoped<GetRepositoriesStrategy, DatabaseStrategy>();
        }

        /// <summary>
        ///     Configure the pipeline for tests
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        #endregion
    }
}