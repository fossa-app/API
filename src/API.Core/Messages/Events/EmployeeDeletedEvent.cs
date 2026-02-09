using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeDeletedEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    CompanyId CompanyId)
  : CompanyEmployeeEvent<Guid, Guid>(TenantID, CompanyId, UserID, EmployeeId);
