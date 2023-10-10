using Pb.Geo.Service.Setup;
using Pb.Profile.Service.Setup;
using Pb.Rate.Service.Setup;
using Pb.Search.Service.Setup;

namespace Pb.ApiGateway.Setup;

public static class GatewayRegistry
{
    public static IServiceCollection SetupGatewayRegistry(this IServiceCollection services, IConfiguration config)
    {
        services.SetupSearchServices(config);
        services.SetupProfileServices(config);
        services.SetupGeoServices(config);
        services.SetupRateServices(config);
        
        return services;
    }
}
