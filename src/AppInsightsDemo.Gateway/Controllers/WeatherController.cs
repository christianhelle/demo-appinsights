using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AppInsightsDemo.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppInsightsDemo.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IOptions<ServiceOptions> options;

        public WeatherController(ILogger<WeatherController> logger, IOptions<ServiceOptions> options)
        {
            _logger = logger;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [HttpGet]
        public async Task<IEnumerable<object>> Get()
        {
            var httpClient = new HttpClient { BaseAddress = options.Value.GetWeatherServiceEndpoint() };
            var serviceClient = new ApiClient(httpClient);

            for (int i = 0; i < 10; i++)
                await serviceClient.GetWeatherForecastAsync();

            return await serviceClient.GetWeatherForecastAsync();
        }
    }
}
