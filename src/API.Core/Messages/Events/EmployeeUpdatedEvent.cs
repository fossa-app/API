using Fossa.API.Core.Entities;
using TIKSN.Data;

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
  : ICompanyEmployeeEvent<Guid, Guid>;
