﻿using Fossa.API.Core.Entities;
using LanguageExt;
using NodaTime;

namespace Fossa.API.Core.Messages.Commands;

public record BranchModificationCommand(
  BranchId ID,
  Guid TenantID,
  Guid UserID,
  string Name,
  DateTimeZone TimeZone)
: EntityTenantCommand<BranchEntity, BranchId, Guid>(TenantID)
{
  public override IEnumerable<BranchId> AffectingTenantEntitiesIdentities
    => Prelude.Seq1(ID);
}
