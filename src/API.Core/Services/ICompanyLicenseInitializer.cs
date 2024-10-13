using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Services;

public interface ICompanyLicenseInitializer
{
  Task InitializeAsync(CompanyId companyId, CancellationToken cancellationToken);
}
