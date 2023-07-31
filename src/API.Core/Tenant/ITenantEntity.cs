using TIKSN.Data;

namespace Fossa.API.Core.Tenant;

public interface ITenantEntity<TEntityIdentity, TTenantIdentity>
  : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
