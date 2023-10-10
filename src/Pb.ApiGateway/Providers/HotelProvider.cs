using Pb.Common.Models;
using Pb.Profile.Service.Services;
using Pb.Search.Service.Services;
using GeoJsonResponse = Pb.Common.Models.GeoJsonResponse;
using Hotel = Pb.Common.Models.Hotel;

namespace Pb.ApiGateway.Providers;

public interface IHotelProvider
{
    GeoJsonResponse? FetchHotels(HotelParameters parameters);
}

public class HotelProvider : IHotelProvider
{
    private readonly ILogger<HotelProvider> _log;
    private readonly ISearchService _searchService;
    private readonly IProfileService _profileService;

    public HotelProvider(
        ILogger<HotelProvider> log, 
        IProfileService profileService,
        ISearchService searchService)
    {
        _log = log;
        _profileService = profileService;
        _searchService = searchService;
    }
    public GeoJsonResponse? FetchHotels(HotelParameters parameters)
    {
        _log.LogInformation("Checking parameters passed to fetch hotels");
        if (CheckParameters(parameters)) return null;

        try
        {
            var searchResponse = _searchService.Nearby(
                new NearbyRequest
                {
                    Lon = parameters.Lon!.Value,
                    Lat = parameters.Lat!.Value,
                    InDate = parameters.InDate,
                    OutDate = parameters.OutDate
                }) ?? throw new AggregateException();
            
            _log.LogInformation("Successfully Retrieved nearby hotels from search service"); //Add to gRPC

            var profileResponse = _profileService.GetProfiles(
                new ProfileRequest
                {
                    HotelIds = searchResponse.HotelIds 
                });
            
            _log.LogInformation("Successfully Retrieved profiles from profile service"); //Add to gRPC
            
            var hotels = CreateGeoJsonResponse(profileResponse.Hotels);
            return hotels;
        }
        catch (AggregateException e) //Add to grpc
        {
            _log.LogError("One of gRPC services responded with Unavailable status code : {Exception}", e);
            return new GeoJsonResponse();
        }
        catch (Exception e)
        {
            _log.LogError("Unknown exception: {Exception}", e);
            return new GeoJsonResponse();
        }
    }

    private static GeoJsonResponse? CreateGeoJsonResponse(IEnumerable<Hotel> hotels)
    {
        var features = new List<Feature?>();

        foreach (var hotel in hotels)
        {
            features.Add(new Feature
            {
                Type = "Feature",
                Id = hotel.Id,
                Properties = new Properties()
                {
                    Name = hotel.Name,
                    PhoneNumber = hotel.PhoneNumber
                },
                Geometry = new Geometry
                {
                    Type = "Point",
                    Coordinates = new double[]
                    {
                        hotel.Address.Lon.Value, // different from grpc
                        hotel.Address.Lat.Value  // different from grpc
                    }
                }
            });
        }

        return new GeoJsonResponse
        {
            Type = "FeatureCollection",
            Features = features!
        };
    }

    private bool CheckParameters(HotelParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.InDate) || string.IsNullOrWhiteSpace(parameters.OutDate))
        {
            _log.LogError("Please specify proper inDate/outDate params");
            return true;
        }

        if (parameters is { Lon: not null, Lat: not null }) return false;

        _log.LogError("Please specify proper lon/lat params");

        return true;
    }
}