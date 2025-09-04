using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VotingSystem.WPF.Infrastructure;

namespace VotingSystem.WPF.Services.HubServices
{
    public abstract class BaseHubService : IBaseHubService
    {
        protected HubConnection? _hubConnection;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpRequestUtility _httpRequestUtility;
        private readonly IConfiguration _configuration;
        private readonly ITokenStoreService _tokenStoreService;

        protected BaseHubService(JsonSerializerOptions jsonOptions, IHttpRequestUtility httpRequestUtility, IConfiguration configuration, ITokenStoreService tokenStoreService)
        {
            _jsonOptions = jsonOptions;
            _httpRequestUtility = httpRequestUtility;
            _configuration = configuration;
            _tokenStoreService = tokenStoreService;
        }

        protected void InitHub(string hubName)
        {
            string hubBaseUrl = _configuration["HubBaseUrl"]
                    ?? throw new InvalidOperationException("Missing HubBaseUrl in appsettings");

            var fullUri = new Uri(new Uri(hubBaseUrl), hubName);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(fullUri, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var token = _tokenStoreService.AuthToken;

                        if (string.IsNullOrWhiteSpace(token) || _httpRequestUtility.IsAccessTokenExpired(token))
                        {
                            var refreshed = await _httpRequestUtility.RedeemTokenAsync();
                            token = refreshed.AuthToken;
                            _tokenStoreService.AuthToken = token;
                            _tokenStoreService.RefreshToken = refreshed.RefreshToken;
                        }

                        return token!;
                    };
                })
                .AddJsonProtocol(config =>
                {
                    config.PayloadSerializerOptions = _jsonOptions;
                })
                .WithAutomaticReconnect()
                .Build();
        }

        protected async Task ConnectHubAsync()
        {
            if (_hubConnection!.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task DisconnectHubAsync()
        {
            if (_hubConnection!.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}
