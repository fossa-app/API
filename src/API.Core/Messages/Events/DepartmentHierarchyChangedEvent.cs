using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentHierarchyChangedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    Option<DepartmentId> PreviousParentDepartmentId,
    Option<DepartmentId> NewParentDepartmentId)
  : ICompanyEvent<Guid>;
