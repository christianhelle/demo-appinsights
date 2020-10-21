using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AppInsightsDemo.Services.Weather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThrowInternalErrorController : ControllerBase
    {
        private readonly ILogger<ThrowInternalErrorController> _logger;

        public ThrowInternalErrorController(ILogger<ThrowInternalErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public object Explode()
        {
            throw new InvalidOperationException("I do not work");
        }
    }
}
