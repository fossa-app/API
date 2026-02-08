using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeUpdatedEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    string JobTitle,
    Option<BranchId> AssignedBranchId,
    Option<DepartmentId> AssignedDepartmentId,
    Option<EmployeeId> ReportsToId,
    CompanyId CompanyId)
  : CompanyEmployeeEvent<Guid, Guid>(TenantID, CompanyId, UserID, EmployeeId);
