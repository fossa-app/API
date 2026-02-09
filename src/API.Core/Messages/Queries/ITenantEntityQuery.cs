namespace Fossa.API.Core.Messages.Queries;

public interface ITenantEntityQuery<TEntityIdentity, out TTenantIdentity, out TResult>
  : ITenantQuery<TTenantIdentity, TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }
}
