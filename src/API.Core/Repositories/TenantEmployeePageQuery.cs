using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public class TenantEmployeePageQuery : PageQuery
{
  public TenantEmployeePageQuery(
    Guid tenantId,
    string search,
    Page page,
    Option<long> reportsToId,
    bool topLevelOnly) : base(page, estimateTotalItems: true)
  {
    TenantId = tenantId;
    Search = search;
    ReportsToId = reportsToId;
    TopLevelOnly = topLevelOnly;
  }

  public Option<long> ReportsToId { get; }
  public string Search { get; }
  public Guid TenantId { get; }
  public bool TopLevelOnly { get; }
}
