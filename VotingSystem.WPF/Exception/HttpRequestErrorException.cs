using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.Exception
{
    public class HttpRequestErrorException(HttpResponseMessage response)
            : System.Exception($"HTTP request failed with status code {response.StatusCode}")
    {
        public HttpResponseMessage Response { get; } = response ?? throw new ArgumentNullException(nameof(response));
    }
}
