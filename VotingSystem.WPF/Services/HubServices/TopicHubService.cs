using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using VotingSystem.Shared.SignalR.Models;
using VotingSystem.WPF.Infrastructure;

namespace VotingSystem.WPF.Services.HubServices
{
    public class TopicHubService : BaseHubService, ITopicHubService
    {
        public TopicHubService(JsonSerializerOptions jsonOptions, IHttpRequestUtility httpRequestUtility, IConfiguration configuration, ITokenStoreService tokenStoreService) : base(jsonOptions, httpRequestUtility, configuration, tokenStoreService)
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
