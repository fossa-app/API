using System.Globalization;
using NodaTime;

namespace Fossa.API.Core.Services;

public interface IDateTimeZoneLookup
{
  Seq<DateTimeZone> ListRegionalTimeZones(RegionInfo region);

  RegionInfo ResolveTimeZoneRegion(DateTimeZone timeZone);
}
