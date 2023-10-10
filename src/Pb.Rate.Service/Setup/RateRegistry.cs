using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pb.Rate.Service.Loaders;
using Pb.Rate.Service.Services;

namespace Pb.Rate.Service.Setup;

public static class RateRegistry
{
    public static IServiceCollection SetupRateServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IRateService, RateService>();
        services.AddSingleton<IRatePlansLoader>(new RatePlansLoader(config["DATA:INVENTORY"]));
        
        return services;
    }

}