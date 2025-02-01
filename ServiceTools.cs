namespace VerticalSlice;

using System;
using System.Linq;
using System.Reflection;

public interface IServiceSetup
{
    public void RegisterServices(IServiceCollection services);
}

public static class ServiceTools
{
    public static void RegisterAllServices<T>(IServiceCollection services)
    {
        var setupTypes = Assembly.GetAssembly(typeof(T))
            ?.GetTypes()
            .Where(t => typeof(IServiceSetup).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList() ?? Enumerable.Empty<Type>();

        foreach (var type in setupTypes)
        {
            if (Activator.CreateInstance(type) is IServiceSetup setupInstance)
            {
                setupInstance.RegisterServices(services);
            }
        }
    }
}