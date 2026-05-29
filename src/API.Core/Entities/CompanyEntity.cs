using TIKSN.Data;
using TIKSN.Globalization;

namespace Fossa.API.Core.Entities;

public record CompanyEntity(
    CompanyId ID,
    Guid TenantID,
    string Name,
    CountryInfo Country)
  : ITenantEntity<CompanyId, Guid>;
