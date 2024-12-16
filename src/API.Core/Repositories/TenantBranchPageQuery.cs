using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public class TenantBranchPageQuery : PageQuery
{
  public Guid TenantId { get; }
  public string Search { get; }

  public TenantBranchPageQuery(
    Guid tenantId,
    string search,
    Page page) : base(page, estimateTotalItems: true)
  {
    TenantId = tenantId;
    Search = search;
  }
}
