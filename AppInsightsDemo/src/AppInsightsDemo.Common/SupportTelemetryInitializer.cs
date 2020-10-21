using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace AppInsightsDemo.Common
{
    public class SupportTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor context;
        private readonly IReadOnlyList<string> headerKeys;

        public SupportTelemetryInitializer(IHttpContextAccessor context, string[] headerKeys = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.headerKeys = TelemetryHeaders.HeaderKeys.Union(headerKeys ?? Array.Empty<string>()).ToList();
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is ISupportProperties supportProperties))
                return;

            foreach (var headerKey in headerKeys)
            {
                var value = TryGetValue(headerKey);
                if (!string.IsNullOrWhiteSpace(value))
                    supportProperties.Properties[headerKey] = value;
            }
        }

        private string TryGetValue(string headerKey)
        {
            var httpContext = context?.HttpContext;
            var request = httpContext?.Request;
            var headers = request?.Headers;
            return headers?.TryGetValue(headerKey, out var value) ?? false
                ? value.FirstOrDefault()
                : null;
        }
    }

    public static class TelemetryHeaders
    {
        public static readonly IReadOnlyList<string> HeaderKeys
            = new[] {
                WellKnownHttpHeaders.SupportKey,
                WellKnownHttpHeaders.CorrelationId,
                WellKnownHttpHeaders.RequestId
            };
    }
}
