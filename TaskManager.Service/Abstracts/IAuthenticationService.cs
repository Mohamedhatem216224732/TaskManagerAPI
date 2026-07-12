using Project_Task_Management.Data.Entities.Identity;
using Project_Task_Management.Data.Helpers;


namespace TaskManager.Service.Abstracts
{
    public interface IAuthenticationService
    {
        public Task<JwtAuthResult> GetJWTToken(ApplicationUser user);

    }
}
