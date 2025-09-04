using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VotingSystem.SignalR.Services;

namespace VotingSystem.SignalR
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSignalRServices(this IServiceCollection services)
        {
            services.AddSingleton<ITopicsNotificationService, TopicsNotificationService>();

            return services;
        }
    }
}
