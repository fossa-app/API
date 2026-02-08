using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Messages.Commands;

public record DepartmentDeletionCommand(
    DepartmentId ID,
    Guid TenantID,
    Guid UserID)
    : EntityTenantCommand<DepartmentEntity, DepartmentId, Guid>(TenantID)
{
  public override IEnumerable<DepartmentId> TenantEntityReferencesIdentities
      => Seq1(ID);
}
