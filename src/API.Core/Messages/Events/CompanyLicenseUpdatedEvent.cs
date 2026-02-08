using Fossa.API.Core.Entities;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Events;

public record CompanyLicenseUpdatedEvent(
    Guid TenantID,
    CompanyId CompanyId,
    License<CompanyEntitlements> license)
  : CompanyEvent<Guid>(TenantID, CompanyId);
