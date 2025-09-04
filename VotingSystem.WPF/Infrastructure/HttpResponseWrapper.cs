using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.Infrastructure
{
    public class HttpResponseWrapper<T>(T response, HttpResponseHeaders headers)
    {
        public T Response { get; set; } = response;
        public HttpResponseHeaders Headers { get; set; } = headers;
    }
}
