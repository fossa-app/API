using Fossa.API.Core.Tenant;

namespace Fossa.API.Core.Entities;

public record TenantEntity<TEntityIdentity, TTenantIdentity>(
  TEntityIdentity ID,
  TTenantIdentity TenantID)
  : ITenantEntity<TEntityIdentity, TTenantIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;
