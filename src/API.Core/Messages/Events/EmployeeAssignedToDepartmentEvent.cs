using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeAssignedToDepartmentEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    CompanyId CompanyId,
    Option<DepartmentId> PreviousDepartmentId,
    Option<DepartmentId> NewDepartmentId)
  : ICompanyEmployeeEvent<Guid, Guid>;
