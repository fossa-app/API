namespace Fossa.API.Core.Messages.Commands;

public record CompanyDeletionCommand(
  long ID,
  Guid TenantID)
  : ITenantCommand<Guid>;
