using Pb.Common.Models;
using Pb.Profile.Service.Models;

namespace Pb.Profile.Service.Services;

public interface IProfileService
{
    public ProfileResult GetProfiles(ProfileRequest request);
}
public class ProfileService : IProfileService
{
    private readonly IDictionary<string?, Hotel> _profiles;

    public ProfileService(IHotelLoader hotelLoader)
    {
        _profiles = InitializeProfiles(hotelLoader.Hotels);
    }

    public ProfileResult GetProfiles(ProfileRequest request)
    {
        var result = new ProfileResult()
        {
            Hotels = new List<Hotel?>()
        };

        foreach (var hotelId in request.HotelIds)
        {
            if (_profiles.TryGetValue(hotelId, out var hotelProfile))
            {
                result.Hotels.Add(hotelProfile);
            }
        }

        return result;
    }

    private Dictionary<string?, Hotel> InitializeProfiles(IEnumerable<Hotel> hotels)
    {
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