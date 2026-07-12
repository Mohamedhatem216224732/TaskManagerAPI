using Project_Task_Management.Data.Entities.Identity;
using TaskManager.Infrastructure.InfrastructureBases;

namespace TaskManager.Infrastructure.Abstracts
{
    public interface IRefershTokenRepository : IGenericRepositoryAsync<UserRefreshToken>
    {
    }
}
