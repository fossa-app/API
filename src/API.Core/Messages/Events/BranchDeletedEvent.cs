using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record BranchDeletedEvent(
    Guid TenantID,
    BranchId BranchId,
    CompanyId CompanyId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
