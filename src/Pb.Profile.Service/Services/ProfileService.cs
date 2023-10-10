using Microsoft.Extensions.Logging;
using Pb.Common.Models;
using Pb.Profile.Service.Models;

namespace Pb.Profile.Service.Services;

public interface IProfileService
{
    public ProfileResult GetProfiles(ProfileRequest request);
}
public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _log;
    private readonly IDictionary<string?, Hotel> _profiles;

    public ProfileService(ILogger<ProfileService> log, IHotelLoader hotelLoader)
    {
        _log = log;
        _profiles = InitializeProfiles(hotelLoader.Hotels);
    }

    public ProfileResult GetProfiles(ProfileRequest request)
    {
        if (request.HotelIds is null && request?.HotelIds?.Count() == 0)
        {
            _log.LogWarning("Request contained empty sequence of hotelIds, you need to specify them in the request body of POST method");
            return new ProfileResult();
        }
        #if DEBUG
        _log.LogDebug("Getting profiles for hotels with IDs (first 10 entries): {hotels}", request?.HotelIds?.Take(10));
        #endif

        return new ProfileResult()
            {
                Hotels =
                    _profiles
                        .Where(p => request.HotelIds.Contains(p.Key))
                        .Select(p => p.Value)
            };
    }

    private Dictionary<string?, Hotel> InitializeProfiles(IEnumerable<Hotel> hotels)
    {
        _log.LogInformation("Initializing hotel profiles...");
        
        var profiles = new Dictionary<string?, Hotel>();

        foreach (var hotel in hotels)
        {
            profiles[hotel.Id] = hotel;
        }

        if (profiles.Count == 0)
        {
            throw new NullReferenceException("No data was loaded to the memory, please provide it (Data/hotels.json)");
        }
        
        return profiles;
    }
}