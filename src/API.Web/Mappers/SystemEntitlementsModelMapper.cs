using System.Globalization;
using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class SystemEntitlementsModelMapper : IMapper<SystemEntitlements, SystemEntitlementsModel>
{
  private readonly IHostEnvironment _hostEnvironment;
  private readonly IMapper<RegionInfo, CountryModel> _countryModelMapper;

  public SystemEntitlementsModelMapper(
    IHostEnvironment hostEnvironment,
    IMapper<RegionInfo, CountryModel> countryModelMapper)
  {
    _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
    _countryModelMapper = countryModelMapper ?? throw new ArgumentNullException(nameof(countryModelMapper));
  }

  public SystemEntitlementsModel Map(SystemEntitlements source)
  {
    return new SystemEntitlementsModel(
      _hostEnvironment.EnvironmentName,
      source.EnvironmentName.ToString() ?? string.Empty,
      source.Countries.Map(_countryModelMapper.Map).ToList(),
      source.MaximumCompanyCount);
  }
}
