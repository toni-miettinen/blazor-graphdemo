using VerticalSlice.Features.Weather.Domain;
using VerticalSlice.Infra;

namespace VerticalSlice.Features.Weather;

public interface IRepository
{
    Task<IEnumerable<WeatherForecast>> Get();
    Task Add(WeatherForecast forecast);
    Task EnsureData();
}

public class RedisRepository : IRepository
{
    private RedisConnection _connection;
    public RedisRepository(RedisConnection connection)
    {
        _connection = connection;
    }

    public async Task EnsureData()
    {
        if (!(await Get()).Any())
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToArray();
            foreach (var forecast in forecasts)
            {
                await Add(forecast);
            }
        }
    }

    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var data = await _connection.Database.ListRangeAsync("weather", 0, -1);
        var forecasts = data
            .Select(x => Serialization.Deserialize<WeatherForecast>(x.ToString()))
            .OfType<WeatherForecast>()
            .ToArray();
        return forecasts;
    }

    public async Task Add(WeatherForecast forecast)
    {
        await _connection.Database.ListLeftPushAsync("weather", Serialization.Serialize(forecast));
    }
}

public class MockRepository : IRepository
{
    private List<WeatherForecast> _forecasts = new();

    public Task EnsureData()
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        _forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToList();
        return Task.CompletedTask;
    }

    public Task Add(WeatherForecast forecast)
    {
        _forecasts.Add(forecast);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<WeatherForecast>> Get() => Task.FromResult<IEnumerable<WeatherForecast>>(_forecasts);
}