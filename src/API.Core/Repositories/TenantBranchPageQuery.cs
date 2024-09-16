﻿using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public class TenantBranchPageQuery : PageQuery
{
  public Guid TenantId { get; }

  public TenantBranchPageQuery(
    Guid tenantId,
    Page page) : base(page, estimateTotalItems: true)
  {
    TenantId = tenantId;
  }
}
