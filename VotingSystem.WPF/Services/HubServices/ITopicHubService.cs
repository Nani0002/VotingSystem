using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.SignalR.Models;

namespace VotingSystem.WPF.Services.HubServices
{
    public interface ITopicHubService
    {
        public Task StartHubConnectionAsync(int topicId);
        public Task DisconnectHubAsync(int topicId);

        public event Action<TopicNotificationDto>? OnVoteUpdateReceived;
    }
}
