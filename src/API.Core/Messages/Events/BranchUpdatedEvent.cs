using Fossa.API.Core.Entities;
using NodaTime;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Events;

public record BranchUpdatedEvent(
    Guid TenantID,
    BranchId BranchId,
    CompanyId CompanyId,
    string Name,
    DateTimeZone TimeZone,
    Option<Address> Address)
  : ICompanyEvent<Guid>;
