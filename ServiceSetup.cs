namespace VerticalSlice;

using System;
using System.Linq;
using System.Reflection;

public static class ServiceSetup
{
    public static void RegisterServices<T>(IServiceCollection services)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(T)) ?? throw new Exception("no assembly found");

        Type featureSetup = typeof(IServiceSetup);
        var staticClasses = assembly.GetTypes()
            .Where(t => t.IsAssignableFrom(featureSetup) && t.IsClass)
            .ToList();

        foreach (var type in staticClasses)
        {
            Console.WriteLine(type.FullName);
        }
    }
}