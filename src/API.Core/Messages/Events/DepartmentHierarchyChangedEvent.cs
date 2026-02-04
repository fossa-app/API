using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentHierarchyChangedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    Option<DepartmentId> PreviousParentDepartmentId,
    Option<DepartmentId> NewParentDepartmentId)
  : ICompanyEvent<Guid>;
