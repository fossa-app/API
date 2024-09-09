using Fossa.API.Core.Entities;
using static LanguageExt.Prelude;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyModificationCommand(
    CompanyId ID,
    Guid TenantID,
    string Name)
  : EntityTenantCommand<CompanyEntity, CompanyId, Guid>(TenantID)
{
  public override IEnumerable<CompanyId> AffectingTenantEntitiesIdentities
    => Seq1(ID);
}
