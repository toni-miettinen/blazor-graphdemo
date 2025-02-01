using System.Windows.Input;
using VerticalSlice.Features.Weather.Domain;
using MediatR;

namespace VerticalSlice.Features.Weather;

public static class Commands
{
    public record AddWeatherInfo(WeatherForecast Forecast) : IRequest<bool>;

    public class AddWeatherInfoHandler(Repository repo) : IRequestHandler<AddWeatherInfo, bool>
    {
        public async Task<bool> Handle(AddWeatherInfo request, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            repo.Add(request.Forecast);
            return true;
        }
    }
}