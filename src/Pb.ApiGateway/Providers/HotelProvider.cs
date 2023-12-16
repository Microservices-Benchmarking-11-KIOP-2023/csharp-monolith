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
    private readonly ISearchService _searchService;
    private readonly IProfileService _profileService;

    public HotelProvider(
        IProfileService profileService,
        ISearchService searchService)
    {
        _profileService = profileService;
        _searchService = searchService;
    }
    public GeoJsonResponse? FetchHotels(HotelParameters parameters)
    {
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
            
            var profileResponse = _profileService.GetProfiles(
                new ProfileRequest
                {
                    HotelIds = searchResponse.HotelIds 
                });
            
            var hotels = CreateGeoJsonResponse(profileResponse.Hotels);
            return hotels;
        }
        catch (AggregateException e)
        {
            return new GeoJsonResponse();
        }
        catch (Exception e)
        {
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
            return true;
        }

        if (parameters is { Lon: not null, Lat: not null }) return false;
        
        return true;
    }
}