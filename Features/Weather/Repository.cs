using MediatR;
using VerticalSlice.Features.Weather.Domain;

namespace VerticalSlice.Features.Weather;

public interface IRepository
{
    Task<IEnumerable<WeatherForecast>> Get();
    Task Add(WeatherForecast forecast);
}

public class MockRepository : IRepository
{
    private readonly List<WeatherForecast> _forecasts;

    public MockRepository()
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        _forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToList();
    }

    public Task Add(WeatherForecast forecast)
    {
        _forecasts.Add(forecast);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<WeatherForecast>> Get() => Task.FromResult<IEnumerable<WeatherForecast>>(_forecasts);
}