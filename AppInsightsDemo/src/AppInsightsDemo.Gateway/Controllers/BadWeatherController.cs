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
        private readonly IOptions<ServiceOptions> options;

        public BadWeatherController(
            ILogger<BadWeatherController> logger,
            IOptions<ServiceOptions> options)
        {
            _logger = logger;
            this.options = options;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var httpClient = new HttpClient { BaseAddress = options.Value.GetWeatherServiceEndpoint() };
            var serviceClient = new ApiClient(httpClient);

            await serviceClient.GetWeatherForecastAsync();
            await serviceClient.ThrowInternalErrorAsync();
            return "this will never execute...";
        }
    }
}
