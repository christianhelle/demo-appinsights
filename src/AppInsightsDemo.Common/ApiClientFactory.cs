using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AppInsightsDemo.Common
{
    public class HttpClientFactory
    {
        private readonly ServiceOptions options;
        private readonly IHttpContextAccessor context;

        public HttpClientFactory(IOptions<ServiceOptions> options, IHttpContextAccessor context)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.context = context;
        }

        public HttpClient Create()
        {
            var httpClient = new HttpClient { BaseAddress = options.GetWeatherServiceEndpoint() };

            var headers = context.HttpContext.Request.Headers;
            httpClient.DefaultRequestHeaders.Add(
                WellKnownHttpHeaders.CorrelationId, 
                headers.GetOrAddCorrelationId());

            if (headers.TryGetValue(WellKnownHttpHeaders.SupportKey, out var supportKeyValues))
            {
                httpClient.DefaultRequestHeaders.Add(
                    WellKnownHttpHeaders.SupportKey, 
                    supportKeyValues.ToString());
            }

            return httpClient;
        }
    }
}
