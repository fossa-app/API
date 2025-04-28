﻿using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Queries;

public record DepartmentListingQuery(
    Seq<DepartmentId> Ids,
    Guid TenantID,
    Guid UserID)
    : EntityTenantQuery<DepartmentEntity, DepartmentId, Guid, Seq<DepartmentEntity>>(TenantID)
{
  public override IEnumerable<DepartmentId> AffectingTenantEntitiesIdentities
      => Ids;
}
