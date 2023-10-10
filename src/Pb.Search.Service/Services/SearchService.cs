using Microsoft.Extensions.Logging;
using Pb.Common.Models;
using Pb.Geo.Service.Services;
using Pb.Rate.Service.Services;

namespace Pb.Search.Service.Services;

public interface ISearchService
{
    public SearchResult? Nearby(NearbyRequest request);
}

public class SearchService : ISearchService
{
    private readonly ILogger<SearchService> _log;
    private readonly IGeoService _geoService;
    private readonly IRateService _rateService;

    public SearchService(ILogger<SearchService> log, IGeoService geoClient, IRateService rateClient)
    {
        _log = log;
        _geoService = geoClient ?? throw new NullReferenceException("Geo service was not specified. You need to do that before you proceed");
        _rateService = rateClient ?? throw new NullReferenceException("Rate service was not specified. You need to do that before you proceed");;
    }

    public SearchResult? Nearby(NearbyRequest request)
    {
        try
        {
#if DEBUG
            _log.LogInformation("Search service called with parameters: {Request}", request);
            _log.LogInformation("Trying to call Geo service...");
#endif

            var nearbyHotels = _geoService.Nearby(new GeoRequest()
            {
                Lat = request.Lat,
                Lon = request.Lon
            });
            
#if DEBUG
            _log.LogInformation("Successfully retrieved data from Geo Service");
            _log.LogInformation("Trying to call Geo service...");
#endif

            var hotelRates = _rateService.GetRates(new RateRequest()
            {
                HotelIds = nearbyHotels?.HotelIds,
                InDate = request.InDate,
                OutDate = request.OutDate
            }); 
            
#if DEBUG
            _log.LogInformation("Successfully retrieved data from Rates Service");
#endif
            
            return new SearchResult()
            {
                HotelIds = hotelRates.RatePlans.Select(x => x.HotelId)
            };
        }
        catch (Exception e)
        {
            _log.LogError("Invalid response code or parameters: {Exception}", e);
            return null;
        }
    }
}