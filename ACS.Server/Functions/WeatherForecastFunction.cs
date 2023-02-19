using System.Net;
using ACS.Shared;
using JetBrains.Annotations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ACS.Server.Functions;

[UsedImplicitly]
public class WeatherForecastFunction
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private readonly ILogger _logger;

    public WeatherForecastFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<WeatherForecast>();
    }

    [UsedImplicitly]
    [Function("WeatherForecast")]
    public async Task<HttpResponseData> WeatherForecast([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var weatherForecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
            .ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(weatherForecast);
        return response;
    }
}