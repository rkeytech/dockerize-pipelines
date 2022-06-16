using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace MyAPI1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(template: "{error}/{errorType}")]
        public IEnumerable<WeatherForecast> Get(string error, string? errorType)
        {
            var mockError = error.Equals(value: "true");

            _logger.LogInformation($"Executing weather forecast controller with mock error = {mockError}");
            if (mockError)
            {
                switch (errorType)
                {
                    case "argumentNull":
                        throw new ArgumentNullException();
                    case "argumentOutRange":
                        throw new ArgumentOutOfRangeException();
                    case "keyNotFound":
                        throw new KeyNotFoundException();
                    default:
                        throw new ApplicationException();
                }
            }

            return Enumerable.Range(start: 1, count: 5).Select(selector: index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(value: index),
                    TemperatureC = Random.Shared.Next(minValue: -20, maxValue: 55),
                    Summary = Summaries[Random.Shared.Next(maxValue: Summaries.Length)]
                })
                .ToArray();
        }
    }
}