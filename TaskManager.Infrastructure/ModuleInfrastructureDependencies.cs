using Microsoft.Extensions.DependencyInjection;
using TaskManager.Infrastructure.Abstracts;
using TaskManager.Infrastructure.InfrastructureBases;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {

        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient<ITaskManagerRepository, ProjectRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IRefershTokenRepository, RefershTokenRepository>();
            return services;
        }
    }
}
