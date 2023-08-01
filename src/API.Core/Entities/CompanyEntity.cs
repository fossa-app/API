using Fossa.API.Core.Tenant;

namespace Fossa.API.Core.Entities;

public record CompanyEntity(
  long ID,
  Guid TenantID,
  string Name)
  : ITenantEntity<long, Guid>;
