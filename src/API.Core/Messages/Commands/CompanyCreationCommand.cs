using Fossa.API.Core.Entities;
using TIKSN.Globalization;

namespace Fossa.API.Core.Messages.Commands;

public record CompanyCreationCommand(
  Guid TenantID,
  string Name,
  CountryInfo Country)
  : TenantEntityCommand<CompanyEntity, CompanyId, Guid>(TenantID)
{
  public override IEnumerable<CompanyId> TenantEntityIdentities
    => [];
}
