using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pb.Geo.Service.Models;
using Pb.Geo.Service.Services;

namespace Pb.Geo.Service.Setup;

public static class GeoRegistry
{
    public static IServiceCollection SetupGeoServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IGeoService, GeoService>();
        services.AddSingleton<IPointLoader>(new PointLoader(config["DATA:GEO"]));

        return services;
    }
}