using Fossa.API.Core.Entities;
using Fossa.Licensing;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public interface ICompanyLicenseCreator
{
  Task<Validation<Error, License<CompanyEntitlements>>> CreateAsync(
    CompanyId companyId,
    Seq<byte> licenseData,
    CancellationToken cancellationToken);
}
