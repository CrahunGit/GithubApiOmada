using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GithubApiOmada.Features.GetRepositories
{
    public class GithubRepository
    { 
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? LicenseId { get; set; }

        public License? License { get; set; }
    }

    public class License
    {
        public int Id { get; set; }

        [Required]
        public string? Key { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}