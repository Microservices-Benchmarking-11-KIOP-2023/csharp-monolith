using Pb.Common.Models;
using Pb.Geo.Service.Models;

namespace Pb.Geo.Service.Services;

public interface IGeoService
{
    public GeoResult Nearby(GeoRequest request);
}
public class GeoService : IGeoService
{
    private readonly IEnumerable<Point> _points;
    private const int MaxSearchRadius = 10;
    private const double EarthRadius = 6371;
    private const double RadianConst = Math.PI / 180;

    public GeoService(IPointLoader pointLoader)
    {
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
            HotelIds = kNearestNeighbours
        };
    }
    
    private IEnumerable<string> GetNeighbors(Point point, IEnumerable<Point> points)
    {
        return points
            .Select(p => new { Point = p, Distance = CalculateDistanceBetweenPoints(p, point) })
            .Where(pointAndDistance => pointAndDistance.Distance <= MaxSearchRadius)
            .OrderBy(pointAndDistance => pointAndDistance.Distance)
            .Select(pointAndDistance => pointAndDistance.Point.HotelId);
    }
    
    private double CalculateDistanceBetweenPoints(Point p1, Point p2)
    {
        var latDistance = ToRadians(p2.Lat - p1.Lat);
        var lonDistance = ToRadians(p2.Lon - p1.Lon);
        
        var a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                + Math.Cos(ToRadians(p1.Lat)) * Math.Cos(ToRadians(p2.Lat))
                                              * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);

        var centralAngle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadius * centralAngle;
    }

    static double ToRadians(float? degrees)
    {
        return degrees.Value * RadianConst;
    }
}