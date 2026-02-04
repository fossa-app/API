using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Events;

public record DepartmentManagerChangedEvent(
    Guid TenantID,
    DepartmentId DepartmentId,
    CompanyId CompanyId,
    Option<EmployeeId> PreviousManagerId,
    Option<EmployeeId> NewManagerId)
  : ICompanyEvent<Guid>;
