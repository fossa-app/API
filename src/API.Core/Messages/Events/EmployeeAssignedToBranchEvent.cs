using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Events;

public record EmployeeAssignedToBranchEvent(
    Guid TenantID,
    Guid UserID,
    EmployeeId EmployeeId,
    CompanyId CompanyId,
    Option<BranchId> PreviousBranchId,
    Option<BranchId> NewBranchId)
  : ICompanyEmployeeEvent<Guid, Guid>;
