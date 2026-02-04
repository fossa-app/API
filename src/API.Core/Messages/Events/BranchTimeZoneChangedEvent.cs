using Fossa.API.Core.Entities;
using NodaTime;

namespace Fossa.API.Core.Messages.Events;

public record BranchTimeZoneChangedEvent(
    Guid TenantID,
    BranchId BranchId,
    CompanyId CompanyId,
    DateTimeZone PreviousTimeZone,
    DateTimeZone NewTimeZone)
  : ICompanyEvent<Guid>;
