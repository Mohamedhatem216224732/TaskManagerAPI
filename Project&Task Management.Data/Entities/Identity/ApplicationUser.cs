using Microsoft.AspNetCore.Identity;

namespace Project_Task_Management.Data.Entities.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser()
        {
            UserRefreshToken = new HashSet<UserRefreshToken>();
        }


        public string FullName { get; set; } = string.Empty;
        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<UserRefreshToken> UserRefreshToken { get; set; }

    }
}
