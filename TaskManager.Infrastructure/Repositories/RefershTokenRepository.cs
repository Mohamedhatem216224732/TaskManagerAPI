using Microsoft.EntityFrameworkCore;
using Project_Task_Management.Data.Entities.Identity;
using TaskManager.Infrastructure.Abstracts;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.InfrastructureBases;


namespace TaskManager.Infrastructure.Repositories
{
    public class RefershTokenRepository : GenericRepositoryAsync<UserRefreshToken>, IRefershTokenRepository
    {
        #region Fields
        private readonly DbSet<UserRefreshToken> _UserRefreshToken;
        #endregion

        #region Constructors
        public RefershTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _UserRefreshToken = dbContext.Set<UserRefreshToken>();
        }
        #endregion

        #region Handle Functions




        #endregion
    }
}
