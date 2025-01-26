using NodaTime;
using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record BranchEntity(
    BranchId ID,
    Guid TenantID,
    CompanyId CompanyId,
    string Name,
    DateTimeZone TimeZone,
    Option<Address> Address)
  : ITenantEntity<BranchId, Guid>;
