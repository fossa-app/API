using Fossa.Licensing;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public interface ISystemLicenseRetriever
{
  Task<Validation<Error, License<SystemEntitlements>>> GetAsync(CancellationToken cancellationToken);
}
