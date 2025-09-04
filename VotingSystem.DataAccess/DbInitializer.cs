using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess
{
    public static class DbInitializer
    {
        public static void Initialize(VotingSystemDbContext context, UserManager<User>? userManager = null)
        {
            context.Database.Migrate();

            if (userManager != null)
            {
                SeedUsersAsync(userManager).Wait();
            }

            if (context.Topics.Any())
            {
                return;
            }

            var admin = userManager!.FindByEmailAsync("admin@example.com").Result ?? throw new InvalidOperationException("Admin user not found after seeding.");

            string adminId = admin.Id;

            Topic[] topics = [
                new Topic
                {
                    Name = "Menjünk haza?",
                    Description = "Menjünk-e haza most, vagy várjunk még?",
                    StartDate = DateTime.Now,
                    CloseDate = DateTime.Now.AddMinutes(20),
                    MinimalVotes = 1,
                    MaximalVotes = 1,
                    OwnerId = adminId,
                    Choices =
                    [
                        new Choice { Value = "igen" },
                        new Choice { Value = "nem" }
                    ]
                },
                new Topic
                {
                    Name = "Milyen filmet nézzek meg?",
                    Description = "-",
                    StartDate = DateTime.Now,
                    CloseDate = DateTime.Now.AddHours(12),
                    MinimalVotes = 1,
                    MaximalVotes = 2,
                    OwnerId = adminId,
                    Choices =
                    [
                        new Choice { Value = "Film1" },
                        new Choice { Value = "Film2" },
                        new Choice { Value = "Film3" },
                        new Choice { Value = "Film4" }
                    ]
                }
            ];

            context.Topics.AddRange(topics);

            context.SaveChanges();
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // Example to seed an Admin user
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User { UserName = adminEmail, Email = adminEmail, Name = "Test Admin", RefreshToken = Guid.NewGuid() };
                await userManager.CreateAsync(adminUser, adminPassword);
            }
        }
    }
}
