using System.Globalization;
using LanguageExt;
using NodaTime;

namespace Fossa.API.Core.Services;

public interface IDateTimeZoneLookup
{
  Seq<DateTimeZone> ListRegionalTimeZones(RegionInfo region);
}
