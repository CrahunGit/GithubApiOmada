using Microsoft.EntityFrameworkCore;

namespace GithubApiOmada.Infrastructure.Persistence
{
    public class GithubDbContext: DbContext
    {
        public DbSet<GithubRepository> Repositories { get; set; }
        public GithubDbContext(DbContextOptions<GithubDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
