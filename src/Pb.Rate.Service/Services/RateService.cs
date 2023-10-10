using Microsoft.Extensions.Logging;
using Pb.Common.Models;
using Pb.Rate.Service.Loaders;

namespace Pb.Rate.Service.Services;

public interface IRateService
{
    public RateResult GetRates(RateRequest request);
}
public class RateService : IRateService
{
    private readonly ILogger<RateService> _log;
    private readonly IDictionary<(string, string, string), List<RatePlan>> _rateTable;

    public RateService(ILogger<RateService> logger, IRatePlansLoader ratePlansLoader)
    {
        _log = logger;
        _rateTable = InitializeRateTable(ratePlansLoader.RateTable);
    }

    public RateResult GetRates(RateRequest request)
    {
        var result = new RateResult
        {
            RatePlans = new List<RatePlan>()
        };

        foreach (var hotelId in request.HotelIds)
        {
            var stay = (hotelId, request.InDate ?? "", request.OutDate ?? "");

            if (_rateTable.TryGetValue(stay, out var ratePlans))
                result.RatePlans!.AddRange(ratePlans);
        }

        return result;
    }

    private IDictionary<(string, string, string), List<RatePlan>> InitializeRateTable(IEnumerable<RatePlan> ratePlans)
    {
        var rateTable = new Dictionary<(string, string, string), List<RatePlan>>();

        foreach (var ratePlan in ratePlans)
        {
            var stay = (ratePlan.HotelId ?? "", ratePlan.InDate ?? "", ratePlan.OutDate ?? "");
            
            if (stay is ("", "", ""))
                continue;
            
            if (rateTable.TryGetValue(stay, out var existingRatePlans))
                existingRatePlans.Add(ratePlan);
            else
                rateTable[stay] = new List<RatePlan> { ratePlan };
        }

        return rateTable;
    }
}