using System.Globalization;
using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using LanguageExt;
using NodaTime;
using NodaTime.TimeZones;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class SystemEntitlementsModelMapper : IMapper<SystemEntitlements, SystemEntitlementsModel>
{
  private readonly IMapper<RegionInfo, CountryModel> _countryModelMapper;
  private readonly IHostEnvironment _hostEnvironment;
  private readonly IMapper<DateTimeZone, TimeZoneModel> _timeZoneModelMapper;

  public SystemEntitlementsModelMapper(
    IHostEnvironment hostEnvironment,
    IMapper<RegionInfo, CountryModel> countryModelMapper,
    IMapper<DateTimeZone, TimeZoneModel> timeZoneModelMapper)
  {
    _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    _countryModelMapper = countryModelMapper ?? throw new ArgumentNullException(nameof(countryModelMapper));
    _timeZoneModelMapper = timeZoneModelMapper ?? throw new ArgumentNullException(nameof(timeZoneModelMapper));
  }

  public SystemEntitlementsModel Map(SystemEntitlements source)
  {
    var timeZones = source.Countries
      .SelectMany(x => TzdbDateTimeZoneSource.Default.ZoneLocations?.Where(z => string.Equals(x.TwoLetterISORegionName, z.CountryCode, StringComparison.OrdinalIgnoreCase)) ?? [])
      .Select(x => DateTimeZoneProviders.Tzdb[x.ZoneId])
      .ToSeq();

    return new SystemEntitlementsModel(
      _hostEnvironment.EnvironmentName,
      source.EnvironmentName.ToString() ?? string.Empty,
      [.. source.Countries.Map(_countryModelMapper.Map)],
      [.. timeZones.Map(_timeZoneModelMapper.Map).OrderBy(x => x.CurrentOffset)],
      source.MaximumCompanyCount);
  }
}
