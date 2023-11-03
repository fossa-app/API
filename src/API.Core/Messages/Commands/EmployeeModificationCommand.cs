using Fossa.API.Core.Entities;
using LanguageExt;

namespace Fossa.API.Core.Messages.Commands;

public record EmployeeModificationCommand(
    EmployeeId ID,
    Guid TenantID,
    Guid UserID,
    string FirstName,
    string LastName,
    string FullName)
  : EntityTenantCommand<EmployeeEntity, EmployeeId, Guid>(TenantID)
{
  public override IEnumerable<EmployeeId> AffectingTenantEntitiesIdentities
    => Prelude.Seq1(ID);
}
