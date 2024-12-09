using Fossa.API.Core.Services;
using Fossa.API.Web.ApiModels;
using NodaTime;
using NodaTime.TimeZones;
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
      _dateTimeZoneLookup.ResolveTimeZoneRegion(source).TwoLetterISORegionName,
      source.GetUtcOffset(_clock.GetCurrentInstant()).ToTimeSpan());
  }
}
