using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.Models;

namespace VotingSystem.WPF.Infrastructure
{
    public interface IHttpRequestUtility
    {
        Task<HttpResponseWrapper<T>> ExecuteGetHttpRequestAsync<T>(string uri);
        Task<TU?> ExecutePutHttpRequestAsync<T, TU>(string uri, T requestDto);
        Task<TU?> ExecutePostHttpRequestAsync<T, TU>(string uri, T requestDto);
        Task ExecutePostHttpRequestAsync(string uri);
        Task ExecuteDeleteHttpRequestAsync(string uri);
        Task<LoginResponseDto> RedeemTokenAsync(CancellationToken cancellationToken = default);
        bool IsAccessTokenExpired(string token);
    }
}
