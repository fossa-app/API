using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeReportingStructureChangedEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    CompanyId CompanyId,
    Option<EmployeeId> PreviousReportsToId,
    Option<EmployeeId> NewReportsToId)
  : ICompanyEmployeeEvent<Guid, Guid>;
