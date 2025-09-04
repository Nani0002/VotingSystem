using System.Text.Json;
using AutoMapper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using VotingSystem.Blazor.WebAssembly.Config;
using VotingSystem.Blazor.WebAssembly.Infrastructure;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Shared.SignalR.Models;

namespace VotingSystem.Blazor.WebAssembly.Services.HubServices
{
    public class TopicHubService : BaseHubService, ITopicHubService
    {
        public TopicHubService(AppConfig appConfig, JsonSerializerOptions jsonOptions, ILocalStorageService localStorageService, IHttpRequestUtility httpRequestUtility) : base(appConfig, jsonOptions, localStorageService, httpRequestUtility)
        {
        }

        public async Task StartHubConnectionAsync(int topicId)
        {
            InitHub("TopicsHub");

            _hubConnection!.On<TopicNotificationDto>("ReceiveVoteUpdate", dto =>
            {
                OnVoteUpdateReceived?.Invoke(dto);
            });

            await ConnectHubAsync();
            await _hubConnection!.InvokeAsync("JoinTopicsGroup", topicId);
        }

        public async Task DisconnectHubAsync(int topicId)
        {
            await _hubConnection!.InvokeAsync("LeaveTopicGroup", topicId);
            await DisconnectHubAsync();
        }

        public event Action<TopicNotificationDto>? OnVoteUpdateReceived;
    }
}
