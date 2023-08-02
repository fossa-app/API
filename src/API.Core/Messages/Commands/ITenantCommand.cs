namespace Fossa.API.Core.Messages.Commands;

public interface ITenantCommand<out TTenantIdentity> : ICommand
{
  TTenantIdentity TenantID { get; }
}
