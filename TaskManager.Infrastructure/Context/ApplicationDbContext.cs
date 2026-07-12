using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_Task_Management.Data.Entities;
using Project_Task_Management.Data.Entities.Identity;

namespace TaskManager.Infrastructure.Data
{
    public class ApplicationDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);



            var hasher = new PasswordHasher<ApplicationUser>();

            var dummyUser = new ApplicationUser
            {
                Id = 1,
                UserName = "dev_user",
                NormalizedUserName = "DEV_USER",
                Email = "dev@taskmanager.com",
                NormalizedEmail = "DEV@TASKMANAGER.COM",
                EmailConfirmed = true,
                SecurityStamp = "STATIC_SECURITY_STAMP_VALUE_12345"
            };
            dummyUser.PasswordHash = hasher.HashPassword(dummyUser, "P@ssword123");

            modelBuilder.Entity<ApplicationUser>().HasData(dummyUser);

            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    Id = 1,
                    Name = "E-Commerce Platform",
                    Description = "Developing the backend APIs for the new online store.",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UserId = 1
                },
                new Project
                {
                    Id = 2,
                    Name = "Mobile App API",
                    Description = "Building RESTful endpoints for the Flutter application.",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UserId = 1
                }
            );

            modelBuilder.Entity<ProjectTask>().HasData(
                new ProjectTask
                {
                    Id = 1,
                    Title = "Database Design",
                    Description = "Create tables, relations, and initial migrations.",
                    Status = Project_Task_Management.Data.Entities.TaskStatus.Completed,
                    Priority = TaskPriority.High,
                    DueDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                    ProjectId = 1
                },
                new ProjectTask
                {
                    Id = 2,
                    Title = "Implement JWT Authentication",
                    Description = "Secure the API endpoints using Bearer tokens.",
                    Status = Project_Task_Management.Data.Entities.TaskStatus.InProgress,
                    Priority = TaskPriority.High,
                    DueDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                    ProjectId = 1
                },
                new ProjectTask
                {
                    Id = 3,
                    Title = "Integrate Redis Cache",
                    Description = "Cache the project lookup tables for high performance.",
                    Status = Project_Task_Management.Data.Entities.TaskStatus.Pending,
                    Priority = TaskPriority.Medium,
                    DueDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                    ProjectId = 2
                }
            );
        }



    }
}
