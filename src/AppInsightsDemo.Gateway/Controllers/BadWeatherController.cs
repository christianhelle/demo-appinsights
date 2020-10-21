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
    public class BadWeatherController : ControllerBase
    {
        private readonly ILogger<BadWeatherController> _logger;
        private readonly HttpClientFactory httpClientFactory;

        public BadWeatherController(
            ILogger<BadWeatherController> logger,
            HttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var serviceClient = new ApiClient(httpClientFactory.Create());
            await serviceClient.GetWeatherForecastAsync();
            await serviceClient.ThrowInternalErrorAsync();
            return "this will never execute...";
        }
    }
}
