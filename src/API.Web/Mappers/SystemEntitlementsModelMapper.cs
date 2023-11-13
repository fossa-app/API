using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class SystemEntitlementsModelMapper : IMapper<SystemEntitlements, SystemEntitlementsModel>
{
  private readonly IHostEnvironment _hostEnvironment;

  public SystemEntitlementsModelMapper(IHostEnvironment hostEnvironment)
  {
    _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
  }

  public SystemEntitlementsModel Map(SystemEntitlements source)
  {
    return new SystemEntitlementsModel(
      _hostEnvironment.EnvironmentName,
      source.EnvironmentName.ToString() ?? string.Empty,
      source.MaximumCompanyCount);
  }
}
