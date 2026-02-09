namespace Fossa.API.Core.Messages.Commands;

public interface ITenantEntityCommand<TEntityIdentity, out TTenantIdentity>
  : ITenantCommand<TTenantIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }
}
