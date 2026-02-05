using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentUpdatedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    string Name,
    Option<DepartmentId> ParentDepartmentId,
    EmployeeId ManagerId)
  : ICompanyEvent<Guid>;
