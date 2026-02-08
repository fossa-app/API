using Fossa.API.Core.Entities;
using NodaTime;

namespace Fossa.API.Core.Messages.Events;

public record BranchUpdatedEvent(
    Guid TenantID,
    BranchId BranchId,
    CompanyId CompanyId,
    string Name,
    DateTimeZone TimeZone,
    Option<Address> Address)
  : CompanyEvent<Guid>(TenantID, CompanyId);
