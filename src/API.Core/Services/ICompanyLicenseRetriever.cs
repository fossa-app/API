using Fossa.API.Core.Entities;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public interface ICompanyLicenseRetriever
{
  Task<Validation<Error, License<CompanyEntitlements>>> GetAsync(CompanyId companyId, CancellationToken cancellationToken);
}
