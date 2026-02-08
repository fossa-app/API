using TIKSN.Integration.Messages.Commands;

namespace Fossa.API.Core.Messages.Commands;

public interface ITenantCommand<TEntityIdentity, out TTenantIdentity>
  : ICommand
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
