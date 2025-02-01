namespace VerticalSlice.Features.Counter;

public static class ServiceSetup
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<Repository>();
    }
}