using System.Globalization;
using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record CompanyEntity(
    CompanyId ID,
    Guid TenantID,
    string Name,
    RegionInfo Country)
  : ITenantEntity<CompanyId, Guid>;
