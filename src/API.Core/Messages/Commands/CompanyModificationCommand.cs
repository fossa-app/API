namespace Fossa.API.Core.Messages.Commands;

public record CompanyModificationCommand(
  long ID,
  Guid TenantID,
  string Name)
  : ITenantCommand<Guid>;
