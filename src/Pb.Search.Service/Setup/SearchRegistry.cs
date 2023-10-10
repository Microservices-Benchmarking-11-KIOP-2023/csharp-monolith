using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pb.Search.Service.Services;

namespace Pb.Search.Service.Setup;

public static class Setup
{
    public static IServiceCollection SetupSearchServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<ISearchService, SearchService>();
        
        return services;
    }
}