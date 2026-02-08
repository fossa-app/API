using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentDeletedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
