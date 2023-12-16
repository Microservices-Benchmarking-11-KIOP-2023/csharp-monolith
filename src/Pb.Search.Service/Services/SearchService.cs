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
    private readonly IGeoService _geoService;
    private readonly IRateService _rateService;

    public SearchService(IGeoService geoClient, IRateService rateClient)
    {
        _geoService = geoClient ?? throw new NullReferenceException("Geo service was not specified. You need to do that before you proceed");
        _rateService = rateClient ?? throw new NullReferenceException("Rate service was not specified. You need to do that before you proceed");;
    }

    public SearchResult? Nearby(NearbyRequest request)
    {
        try
        {
            var nearbyHotels = _geoService.Nearby(new GeoRequest()
            {
                Lat = request.Lat,
                Lon = request.Lon
            });

            var hotelRates = _rateService.GetRates(new RateRequest()
            {
                HotelIds = nearbyHotels?.HotelIds,
                InDate = request.InDate,
                OutDate = request.OutDate
            }); 
            
            return new SearchResult()
            {
                HotelIds = hotelRates.RatePlans.Select(x => x.HotelId)
            };
        }
        catch (Exception e)
        {
            return null;
        }
    }
}