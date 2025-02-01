using MediatR;
using Microsoft.AspNetCore.Mvc;
using VerticalSlice.Features.Weather.Domain;

namespace VerticalSlice.Features.Weather.Controllers;

[Route("/api/weather")]
public class WeatherController(IMediator mediator, ILogger<WeatherController> logger) : Controller
{
    [HttpGet]
    public async Task<WeatherForecast[]> GetWeather()
    {
        logger.LogInformation("GetWeather");
        var resp = await mediator.Send(new Queries.GetWeatherRequest());
        return resp;
    }

    [HttpPost]
    public async Task<IResult> PostWeather(WeatherForecast request)
    {
        logger.LogInformation("PostWeather");
        var success = await mediator.Send(new Commands.AddWeatherInfo(request));
        return !success ? Results.Problem("could not add weather forecast") : Results.Ok();
    }
}