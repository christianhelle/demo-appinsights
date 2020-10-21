using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppInsightsDemo.Gateway;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppInsightsDemo.Services.Weather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetWeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<GetWeatherForecastController> _logger;

        public GetWeatherForecastController(ILogger<GetWeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
