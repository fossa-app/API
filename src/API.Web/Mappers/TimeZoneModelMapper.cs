using System.Globalization;
using Fossa.API.Core.Services;
using Fossa.API.Web.ApiModels;
using NodaTime;
using NodaTime.TimeZones;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class TimeZoneModelMapper : IMapper<DateTimeZone, TimeZoneModel>
{
  private readonly IClock _clock;
  private readonly IMapper<RegionInfo, CountryModel> _countryModelMapper;
  private readonly IDateTimeZoneLookup _dateTimeZoneLookup;

  public TimeZoneModelMapper(
    IDateTimeZoneLookup dateTimeZoneLookup,
    IMapper<RegionInfo, CountryModel> countryModelMapper,
    IClock clock)
  {
    _dateTimeZoneLookup = dateTimeZoneLookup ?? throw new ArgumentNullException(nameof(dateTimeZoneLookup));
    _countryModelMapper = countryModelMapper ?? throw new ArgumentNullException(nameof(countryModelMapper));
    _clock = clock ?? throw new ArgumentNullException(nameof(clock));
  }

  public TimeZoneModel Map(DateTimeZone source)
  {
    var currentOffset = source.GetUtcOffset(_clock.GetCurrentInstant()).ToTimeSpan();
    return new TimeZoneModel(
      source.Id,
      TzdbDateTimeZoneSource.Default.TzdbToWindowsIds[source.Id],
      _countryModelMapper.Map(_dateTimeZoneLookup.ResolveTimeZoneRegion(source)),
      currentOffset,
      currentOffset.TotalHours,
      currentOffset.Hours,
      currentOffset.Minutes);
  }
}
