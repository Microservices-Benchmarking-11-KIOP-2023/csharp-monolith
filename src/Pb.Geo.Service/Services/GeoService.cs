using Microsoft.Extensions.Logging;
using Pb.Common.Models;
using Pb.Geo.Service.Models;

namespace Pb.Geo.Service.Services;

public interface IGeoService
{
    public GeoResult Nearby(GeoRequest request);
}
public class GeoService : IGeoService
{
    private readonly ILogger<GeoService> _log;
    private readonly IEnumerable<Point> _points;
    private const int MaxSearchRadius = 10;
    private const int MaxSearchResults = Int32.MaxValue;
    private const double EarthRadius = 6371;
    private const double RadianConst = Math.PI / 180;

    public GeoService(ILogger<GeoService> log, IPointLoader pointLoader)
    {
        _log = log;
        _points = pointLoader.Points;
    }

    public GeoResult Nearby(GeoRequest request)
    {
        var center = new Point
        {
            Lat = request.Lat,
            Lon = request.Lon
        };
        
        var kNearestNeighbours = GetNeighbors(center, _points);

        return new GeoResult()
        {
            HotelIds = kNearestNeighbours.Select(x => x.HotelId)
        };
    }
    
    //TODO: Validate if returns proper values.
    //Source: https://stackoverflow.com/questions/65256053/find-k-nearest-neighbor-in-c-sharp
    private IEnumerable<Point> GetNeighbors(Point point, IEnumerable<Point> points)
    {
        return points
            .Select(p => new { Point = p, Distance = CalculateDistanceBetweenPoints(point, p) })
            .Where(pointAndDistance => pointAndDistance.Distance <= Math.Pow(MaxSearchRadius, 2))
            .OrderBy(pointAndDistance => pointAndDistance.Distance)
            .Take(MaxSearchResults)
            .Select(pointAndDistance => pointAndDistance.Point);
    }
    
    private double CalculateDistanceBetweenPoints(Point originPoint, Point destinationPoint)
    {
        var distanceLat = ToRadians(originPoint.Lat - destinationPoint.Lat);
        var distanceLon = ToRadians(originPoint.Lon - originPoint.Lon);

        var inverseHaversine = Math.Pow(Math.Sin(distanceLat / 2), 2) +
                Math.Cos(originPoint.Lat.Value) * Math.Cos(destinationPoint.Lat.Value) *
                Math.Pow(Math.Sin(distanceLon / 2), 2);
        var centralAngle = 2 * Math.Asin(Math.Sqrt(inverseHaversine));

        return centralAngle * EarthRadius;
    }

    static double ToRadians(float? degrees)
    {
        return degrees.Value * RadianConst;
    }
}