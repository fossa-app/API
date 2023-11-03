using Fossa.API.Core.Entities;
using LanguageExt;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeModificationCommand(
    long ID,
    Guid TenantID,
    Guid UserID,
    string FirstName,
    string LastName,
    string FullName)
  : EntityTenantCommand<EmployeeEntity, long, Guid>(TenantID)
{
  public override IEnumerable<long> AffectingTenantEntitiesIdentities
    => Prelude.Seq1(ID);
}
