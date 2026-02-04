using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentCreatedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    string Name,
    Option<DepartmentId> ParentDepartmentId,
    EmployeeId ManagerId)
  : ICompanyEvent<Guid>;
