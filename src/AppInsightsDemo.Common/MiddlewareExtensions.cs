using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AppInsightsDemo.Common
{
    public static class MiddlewareExtensions
    {
        public static void UseInstrumentationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestCorrelationMiddleware>();
            app.UseMiddleware<ExceptionTelemetryMiddleware>();
        }

        public static void AddAppInsightsTelemetry(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<ITelemetryInitializer, SupportTelemetryInitializer>();
        }
    }
}
