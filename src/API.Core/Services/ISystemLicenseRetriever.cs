using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public interface ISystemLicenseRetriever
{
  Task<Validation<Error, License<SystemEntitlements>>> GetAsync(CancellationToken cancellationToken);
}
