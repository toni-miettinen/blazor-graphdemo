using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VerticalSlice.Features.Weather.Domain;

namespace VerticalSlice.Features.Weather;

[ApiController]
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
        logger.LogInformation("PostWeather: {request}", request);

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value?.Errors ?? Enumerable.Empty<ModelError>())
                .Select(x => x.ErrorMessage);
            return Results.BadRequest(string.Join(", ", errors));
        }
        if (request.Date == default)
        {
            return Results.BadRequest();
        }

        var success = await mediator.Send(new Commands.AddWeatherInfo(request));
        return success ? Results.Problem("could not add weather forecast") : Results.Ok();
    }
}