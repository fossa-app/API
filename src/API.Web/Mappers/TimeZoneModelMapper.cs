using Fossa.API.Web.ApiModels;
using NodaTime;
using NodaTime.TimeZones;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class TimeZoneModelMapper : IMapper<DateTimeZone, TimeZoneModel>
{
  private readonly IClock _clock;

  public TimeZoneModelMapper(
    IClock clock)
  {
    _clock = clock ?? throw new ArgumentNullException(nameof(clock));
  }

  public TimeZoneModel Map(DateTimeZone source)
  {
    var currentOffset = source.GetUtcOffset(_clock.GetCurrentInstant()).ToTimeSpan();
    return new TimeZoneModel(
      source.Id,
      TzdbDateTimeZoneSource.Default.TzdbToWindowsIds[source.Id],
      currentOffset,
      currentOffset.TotalHours,
      currentOffset.Hours,
      currentOffset.Minutes);
  }
}
