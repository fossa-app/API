using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record DepartmentDeletionCommand(
    DepartmentId ID,
    Guid TenantID,
    Guid UserID)
    : TenantEntityCommand<DepartmentEntity, DepartmentId, Guid>(TenantID)
{
  public override IEnumerable<DepartmentId> TenantEntityIdentities
      => Seq1(ID);
}
