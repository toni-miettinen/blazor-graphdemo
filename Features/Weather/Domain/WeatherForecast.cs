using System.Text.Json.Serialization;

namespace VerticalSlice.Features.Weather.Domain;

public class WeatherForecast
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
    [JsonPropertyName("temperatureC")]
    public int TemperatureC { get; set; }
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
    [JsonIgnore]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    
    public override string ToString() => $"WeatherForecast {Date} - {TemperatureC} - {Summary}";
}
