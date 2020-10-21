using System;

namespace AppInsightsDemo.Common
{
    public class ServiceOptions
    {
        public EnvironmentType EnvironmentType { get; set; }
        public string EnvironmentName { get; set; } = "Dev";
    }

    public static class ServiceOptionsExtensions
    {
        public static string GetWeatherServiceName(this ServiceOptions serviceOptions)
            => $"jbapi{serviceOptions.EnvironmentName}demoweather";

        public static Uri GetWeatherServiceEndpoint(this ServiceOptions serviceOptions)
        {
            return new Uri(
                serviceOptions.EnvironmentType != EnvironmentType.Local
                    ? $"https://{GetWeatherServiceName(serviceOptions)}.azurewebsites.net"
                    : "https://localhost:63812");
        }
    }

    public enum EnvironmentType
    {
        Local,
        DevTest,
        Production
    }
}
