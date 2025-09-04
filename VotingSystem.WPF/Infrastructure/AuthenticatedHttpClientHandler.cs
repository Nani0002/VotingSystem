using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.Models;
using VotingSystem.WPF.Services;

namespace VotingSystem.WPF.Infrastructure
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly ITokenStoreService _tokenStoreService;

        public AuthenticatedHttpClientHandler(ITokenStoreService tokenStoreService)
        {
            _tokenStoreService = tokenStoreService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_tokenStoreService.AuthToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStoreService.AuthToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshSuccess = await RefreshTokenAsync();

                if (refreshSuccess)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStoreService.AuthToken);
                    response.Dispose();
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }

        private async Task<bool> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(_tokenStoreService.RefreshToken))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Post, "users/refresh")
            {
                Content = JsonContent.Create(_tokenStoreService.RefreshToken)
            };

            var response = await base.SendAsync(request, CancellationToken.None);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (result == null)
                return false;

            _tokenStoreService.AuthToken = result.AuthToken;
            _tokenStoreService.RefreshToken = result.RefreshToken;
            return true;
        }
    }
}
