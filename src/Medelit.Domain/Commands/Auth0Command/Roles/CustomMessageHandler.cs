using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Medelit.Domain.Commands
{
    public class CustomMessageHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("my-custom-header", "my-custom-value");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
