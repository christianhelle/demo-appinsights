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
        private readonly HttpClientFactory httpClientFactory;
        private readonly IOptions<ServiceOptions> options;

        public WeatherController(
            ILogger<WeatherController> logger,
            HttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<object>> Get()
        {
            var serviceClient = new ApiClient(httpClientFactory.Create());
            for (int i = 0; i < 10; i++)
                await serviceClient.GetWeatherForecastAsync();
            return await serviceClient.GetWeatherForecastAsync();
        }
    }
}
