using Microsoft.AspNetCore.Mvc;

namespace MyAPI1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "SomewhatFreeazing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    // ReSharper disable once EmptyConstructor
    public WeatherForecastController()
    {
    }

    [HttpGet("{error}/{errorType}")]
    public IEnumerable<WeatherForecast> Get(string error, string? errorType)
    {
        var mockError = error.Equals("true");

        if (mockError)
            throw errorType switch
            {
                "argumentNull" => new ArgumentNullException(),
                "argumentOutRange" => new ArgumentOutOfRangeException(),
                "keyNotFound" => new KeyNotFoundException(),
                _ => new ApplicationException()
            };

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}