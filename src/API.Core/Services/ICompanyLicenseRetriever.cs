using Fossa.API.Core.Entities;
using Fossa.Licensing;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public interface ICompanyLicenseRetriever
{
  Task<Validation<Error, License<CompanyEntitlements>>> GetAsync(CompanyId companyId, CancellationToken cancellationToken);
}
