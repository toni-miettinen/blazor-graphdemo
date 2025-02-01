namespace VerticalSlice.Features.Weather;

public class ServiceSetup : IServiceSetup
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IRepository, MockRepository>();
    }
}