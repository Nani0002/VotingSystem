using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VotingSystem.Shared.SignalR.HubInterfaces;
using VotingSystem.Shared.SignalR.Models;
using VotingSystem.SignalR.Hubs;

namespace VotingSystem.SignalR.Services
{
    public class TopicsNotificationService : ITopicsNotificationService
    {
        private readonly IHubContext<TopicsHub, ITopicsHub> _hubContext;

        public TopicsNotificationService(IHubContext<TopicsHub, ITopicsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task AddToGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task RemoveFromGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public async Task SendVoteUpdateToGroupAsync(int topicId, TopicNotificationDto dto)
        {
            var group = $"topic-{topicId}";
            await _hubContext.Clients.Group(group).ReceiveVoteUpdate(dto);
        }
    }
}
