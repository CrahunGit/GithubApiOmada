namespace GithubApiOmada.Features.GetStarredRepositories
{
    /// <summary>
    /// Get github repositories from differente places (rest, database, etc)
    /// </summary>
    public interface GetRepositoriesStrategy
    {
        /// <summary>
        /// Current strategy name
        /// </summary>
        Strategy Strategy { get; }

        /// <summary>
        /// Get repositories
        /// </summary>
        /// <param name="githubPersonalToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<GetGithubRepositories.Response>> GetRepositories(string githubPersonalToken, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Get repositories location strategy names
    /// </summary>
    public enum Strategy
    {
        Database,
        GithubRest
    }
}
