﻿using Fossa.API.Core.Entities;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public record DepartmentPagingQuery(
    Guid TenantID,
    Guid UserID,
    string Search,
    Page Page)
    : EntityTenantQuery<DepartmentEntity, DepartmentId, Guid, PageResult<DepartmentEntity>>(TenantID)
    , IPagingQuery<DepartmentEntity>
{
  public override IEnumerable<DepartmentId> AffectingTenantEntitiesIdentities
      => [];
}
