namespace VerticalSlice.Features.Counter;

public class ServiceSetup : IServiceSetup
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IRepository, MockRepository>();
    }
}