using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentCreatedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    string Name,
    Option<DepartmentId> ParentDepartmentId,
    EmployeeId ManagerId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
