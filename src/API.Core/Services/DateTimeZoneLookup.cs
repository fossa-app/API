using System.Collections.Concurrent;
using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.TimeZones;
using TIKSN.Globalization;

namespace Fossa.API.Core.Services;

public class DateTimeZoneLookup : IDateTimeZoneLookup
{
  private readonly IRegionFactory _regionFactory;
  private readonly ConcurrentDictionary<string, Seq<DateTimeZone>> _regionToZonesMap;
  private readonly ConcurrentDictionary<string, RegionInfo> _zoneIdToRegionMap;

  public DateTimeZoneLookup(IRegionFactory regionFactory)
  {
    _regionToZonesMap = new(StringComparer.OrdinalIgnoreCase);
    _zoneIdToRegionMap = new(StringComparer.OrdinalIgnoreCase);
    _regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
  }

  public Seq<DateTimeZone> ListRegionalTimeZones(RegionInfo region)
  {
    return _regionToZonesMap.GetOrAdd(region.TwoLetterISORegionName, ListRegionalTimeZones);
  }

  public RegionInfo ResolveTimeZoneRegion(DateTimeZone timeZone)
  {
    return _zoneIdToRegionMap.GetOrAdd(timeZone.Id, x => ResolveTimeZoneRegion(x));
  }

  private static Seq<DateTimeZone> ListRegionalTimeZones(string twoLetterISORegionName)
  {
    var zoneLocations = TzdbDateTimeZoneSource.Default.ZoneLocations
        ?.Where(x => string.Equals(
          twoLetterISORegionName,
          x.CountryCode,
          StringComparison.OrdinalIgnoreCase)) ?? [];
    return zoneLocations
        .Select(x => DateTimeZoneProviders.Tzdb[x.ZoneId])
        .ToSeq();
  }

  private RegionInfo ResolveTimeZoneRegion(string timeZoneId)
  {
    var zoneLocations = TzdbDateTimeZoneSource.Default.ZoneLocations
        ?.Where(x => string.Equals(
          timeZoneId,
          x.ZoneId,
          StringComparison.OrdinalIgnoreCase)) ?? [];
    return zoneLocations
      .Select(x => _regionFactory.Create(x.CountryCode))
      .Single();
  }
}
