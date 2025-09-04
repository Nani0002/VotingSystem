using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VotingSystem.Shared.SignalR.HubInterfaces;
using VotingSystem.Shared.SignalR.Models;
using VotingSystem.SignalR.Services;

namespace VotingSystem.SignalR.Hubs
{
    public class TopicsHub : Hub<ITopicsHub>
    {
        private readonly ITopicsNotificationService _notificationService;

        public TopicsHub(ITopicsNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task JoinTopicsGroup(int topicId)
        {
            await _notificationService.AddToGroupAsync(Context.ConnectionId, GetTopicGroupName(topicId));
        }

        public async Task LeaveTopicGroup(int topicId)
        {
            await _notificationService.RemoveFromGroupAsync(Context.ConnectionId, GetTopicGroupName(topicId));
        }

        public async Task VoteStatusChanged(int topicId, TopicNotificationDto topicNotificationDto)
        {
            await _notificationService.SendVoteUpdateToGroupAsync(topicId, topicNotificationDto);
        }

        private static string GetTopicGroupName(int topicId) => $"topic-{topicId}";
    }
}
