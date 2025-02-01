using VerticalSlice.Features.Weather.Domain;

namespace VerticalSlice.Features.Weather;

public class Repository
{
    private readonly List<WeatherForecast> _forecasts;

    public Repository()
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
    
    public void Add(WeatherForecast forecast) => _forecasts.Add(forecast);
    public IEnumerable<WeatherForecast> Get() => _forecasts;
}