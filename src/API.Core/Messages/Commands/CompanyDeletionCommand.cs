using Fossa.API.Core.Entities;
using static LanguageExt.Prelude;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyDeletionCommand(
  long ID,
  Guid TenantID)
  : EntityTenantCommand<CompanyEntity, long, Guid>(TenantID)
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
  => Seq1(ID);
}
