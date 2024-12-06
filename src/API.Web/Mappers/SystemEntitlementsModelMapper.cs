using System.Globalization;
using Fossa.API.Core.Services;
using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using LanguageExt;
using NodaTime;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class SystemEntitlementsModelMapper : IMapper<SystemEntitlements, SystemEntitlementsModel>
{
  private readonly IMapper<RegionInfo, CountryModel> _countryModelMapper;
  private readonly IDateTimeZoneLookup _dateTimeZoneLookup;
  private readonly IHostEnvironment _hostEnvironment;
  private readonly IMapper<DateTimeZone, TimeZoneModel> _timeZoneModelMapper;

  public SystemEntitlementsModelMapper(
    IHostEnvironment hostEnvironment,
    IDateTimeZoneLookup dateTimeZoneLookup,
    IMapper<RegionInfo, CountryModel> countryModelMapper,
    IMapper<DateTimeZone, TimeZoneModel> timeZoneModelMapper)
  {
    _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    _dateTimeZoneLookup = dateTimeZoneLookup ?? throw new ArgumentNullException(nameof(dateTimeZoneLookup));
    _countryModelMapper = countryModelMapper ?? throw new ArgumentNullException(nameof(countryModelMapper));
    _timeZoneModelMapper = timeZoneModelMapper ?? throw new ArgumentNullException(nameof(timeZoneModelMapper));
  }

  public SystemEntitlementsModel Map(SystemEntitlements source)
  {
    var timeZones = source.Countries
      .SelectMany(x => _dateTimeZoneLookup.ListRegionalTimeZones(x))
      .ToSeq();

    return new SystemEntitlementsModel(
      _hostEnvironment.EnvironmentName,
      source.EnvironmentName.ToString() ?? string.Empty,
      [.. source.Countries.Map(_countryModelMapper.Map)],
      [.. timeZones.Map(_timeZoneModelMapper.Map).OrderBy(x => x.CurrentOffset)],
      source.MaximumCompanyCount);
  }
}
