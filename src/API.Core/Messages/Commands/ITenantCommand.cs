using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Core.Messages.Commands;

public interface ITenantCommand<out TTenantIdentity>
  : ICommand
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
