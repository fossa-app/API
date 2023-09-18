using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public class TenantEmployeePageQuery : PageQuery
{
  public Guid TenantId { get; }

  public TenantEmployeePageQuery(
    Guid tenantId,
    Page page) : base(page, estimateTotalItems: true)
  {
    TenantId = tenantId;
  }
}
