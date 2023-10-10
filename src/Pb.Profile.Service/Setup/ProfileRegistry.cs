using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pb.Profile.Service.Models;
using Pb.Profile.Service.Services;

namespace Pb.Profile.Service.Setup;

public static class ProfileRegistry
{
    public static IServiceCollection SetupProfileServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<IHotelLoader>(new HotelLoader(config["DATA:HOTELS"]));
        
        return services;
    }
}