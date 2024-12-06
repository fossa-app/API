using System.Collections.Concurrent;
using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.TimeZones;

namespace Fossa.API.Core.Services;

public class DateTimeZoneLookup : IDateTimeZoneLookup
{
  private readonly ConcurrentDictionary<string, Seq<DateTimeZone>> _regionToZonesMap;

  public DateTimeZoneLookup()
  {
    _regionToZonesMap = new(StringComparer.OrdinalIgnoreCase);
  }

  public Seq<DateTimeZone> ListRegionalTimeZones(RegionInfo region)
  {
    return _regionToZonesMap.GetOrAdd(region.TwoLetterISORegionName, ListRegionalTimeZones);
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
}
