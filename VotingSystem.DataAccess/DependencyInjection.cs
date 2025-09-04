using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;

namespace VotingSystem.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<VotingSystemDbContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
            );
            services.AddScoped<DbResetter>();

            services.AddIdentity<User, UserRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<VotingSystemDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<ITopicsService, TopicsService>();
            services.AddScoped<IUsersService, UsersService>();

            return services;
        }
    }
}
