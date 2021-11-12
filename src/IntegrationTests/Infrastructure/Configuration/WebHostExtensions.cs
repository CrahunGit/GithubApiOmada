#region "Namespace imports"

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

#endregion

namespace IntegrationTests.Infrastructure.Configuration
{
    public static class WebHostExtensions
    {
        #region "Static methods"

        public static IWebHost SeedDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (IServiceScope scope = webHost.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                TContext context = services.GetService<TContext>();
                seeder(context, services);
            }

            return webHost;
        }
        #endregion
    }
}