using VerticalSlice.Features.Weather.Domain;

namespace VerticalSlice.Features.Weather;

public static class Queries
{
    public class GetWeatherRequest : MediatR.IRequest<WeatherForecast[]>;

    public class GetWeatherHandler(IRepository repo) : MediatR.IRequestHandler<GetWeatherRequest, WeatherForecast[]>
    {
        public async Task<WeatherForecast[]> Handle(GetWeatherRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            return (await repo.Get()).ToArray();
        }
    }
}