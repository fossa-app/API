using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeAssignedToDepartmentEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    CompanyId CompanyId,
    Option<DepartmentId> PreviousDepartmentId,
    Option<DepartmentId> NewDepartmentId)
  : ICompanyEmployeeEvent<Guid, Guid>;
