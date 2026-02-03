using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Events;

public record BranchAddressChangedEvent(
    Guid TenantID,
    BranchId BranchId,
    CompanyId CompanyId,
    Option<Address> PreviousAddress,
    Option<Address> NewAddress)
  : ICompanyEvent<Guid>;
