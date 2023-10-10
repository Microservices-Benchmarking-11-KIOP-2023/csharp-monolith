using Microsoft.AspNetCore.Mvc;
using Pb.ApiGateway.Providers;
using Pb.Common.Models;

namespace Pb.ApiGateway.Controllers;

public class HotelsController : ControllerBase
{
    private readonly IHotelProvider _hotelProvider;
    
    public HotelsController(IHotelProvider hotelProvider)
    {
        _hotelProvider = hotelProvider;
    }
    
    [HttpGet("/hotels")]
    public GeoJsonResponse? FetchHotels([FromQuery] HotelParameters hotelParameters)
    {
        return _hotelProvider.FetchHotels(hotelParameters);
    }
}