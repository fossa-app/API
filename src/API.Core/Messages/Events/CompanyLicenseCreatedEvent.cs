using Fossa.API.Core.Entities;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Events;

public record CompanyLicenseCreatedEvent(
    Guid TenantID,
    CompanyId CompanyId,
    License<CompanyEntitlements> license)
  : ICompanyEvent<Guid>;
