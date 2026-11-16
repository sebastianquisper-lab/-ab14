using Microsoft.Extensions.DependencyInjection;

namespace SQR.Applicattion.Configuration;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(ApplicationAssemblyMarker).Assembly));

        return services;
    }
}
