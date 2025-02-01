namespace VerticalSlice.Features.Weather;

public static class ServiceSetup
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<Repository>();
    }
}