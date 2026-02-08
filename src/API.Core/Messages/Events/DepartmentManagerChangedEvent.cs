using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentManagerChangedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    Option<EmployeeId> PreviousManagerId,
    Option<EmployeeId> NewManagerId)
  : CompanyEvent<Guid>(TenantID, CompanyId);
