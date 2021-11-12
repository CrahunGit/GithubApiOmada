using GithubApiOmada.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GithubApiOmada.Features.GetRepositories
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class DatabaseStrategy : GetRepositoriesStrategy
    {
        private readonly GithubDbContext _database;

        public Strategy Strategy { get; } = Strategy.Database;

        public DatabaseStrategy(GithubDbContext database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<List<GetGithubRepositories.Response>> GetRepositories(string githubPersonalToken, CancellationToken cancellationToken)
        {
            return await GetRepositoriesFromDatabase(cancellationToken);
        }

        /// <summary>
        /// Get repositories response from database
        /// </summary>
        /// <param name="response">Read repositories from github api</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<List<GetGithubRepositories.Response>> GetRepositoriesFromDatabase(CancellationToken cancellationToken)
        {
            return await _database.Repositories.Select(m => new GetGithubRepositories.Response(
                m.Id,
                m.Name,
                m.License != null ? new GetGithubRepositories.License(m.License.Key, m.License.Name) : null)
            ).ToListAsync(cancellationToken);
        }
    }
}
