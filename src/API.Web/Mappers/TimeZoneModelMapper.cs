using Fossa.Bridge.Models.ApiModels;
using NodaTime;
using NodaTime.TimeZones;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class TimeZoneModelMapper : IMapper<DateTimeZone, TimeZoneModel>
{
  private readonly IClock _clock;
  private readonly IDateTimeZoneLookup _dateTimeZoneLookup;

  public TimeZoneModelMapper(
    IDateTimeZoneLookup dateTimeZoneLookup,
    IClock clock)
  {
    _dateTimeZoneLookup = dateTimeZoneLookup ?? throw new ArgumentNullException(nameof(dateTimeZoneLookup));
    _clock = clock ?? throw new ArgumentNullException(nameof(clock));
  }

  public TimeZoneModel Map(DateTimeZone source)
  {
    return new TimeZoneModel(
      source.Id,
      TzdbDateTimeZoneSource.Default.TzdbToWindowsIds[source.Id],
      _dateTimeZoneLookup.ResolveTimeZoneCountry(source).PrincipalRegion.TwoLetterISORegionName,
      source.GetUtcOffset(_clock.GetCurrentInstant()).ToTimeSpan());
  }
}
