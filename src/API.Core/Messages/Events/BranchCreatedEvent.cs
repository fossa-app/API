using Fossa.API.Core.Entities;
using NodaTime;

namespace Fossa.API.Core.Messages.Events;

public record BranchCreatedEvent(
    Guid TenantID,
    BranchId BranchId,
    CompanyId CompanyId,
    string Name,
    DateTimeZone TimeZone,
    Option<Address> Address)
  : ICompanyEvent<Guid>;
