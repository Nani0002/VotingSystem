using VotingSystem.Shared.SignalR.Models;

namespace VotingSystem.Blazor.WebAssembly.Services.HubServices
{
    public interface ITopicHubService
    {
        public Task StartHubConnectionAsync(int topicId);
        public Task DisconnectHubAsync(int topicId);

        public event Action<TopicNotificationDto>? OnVoteUpdateReceived;
    }
}
