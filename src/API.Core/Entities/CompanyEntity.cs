using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record CompanyEntity(
    CompanyId ID,
    Guid TenantID,
    string Name)
  : ITenantEntity<CompanyId, Guid>;
