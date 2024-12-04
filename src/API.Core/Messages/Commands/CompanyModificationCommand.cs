using System.Globalization;
using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyModificationCommand(
    Guid TenantID,
    string Name,
    RegionInfo Country)
  : EntityTenantCommand<CompanyEntity, CompanyId, Guid>(TenantID)
{
  public override IEnumerable<CompanyId> AffectingTenantEntitiesIdentities
    => [];
}
