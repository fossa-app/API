using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public abstract record EntityTenantQuery<TEntity, TEntityIdentity, TTenantIdentity, TResult>(
  TTenantIdentity TenantID)
  : ITenantQuery<TEntityIdentity, TTenantIdentity, TResult>, ITenantEntityReferences<TEntityIdentity>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  public abstract IEnumerable<TEntityIdentity> AffectingTenantEntitiesIdentities { get; }

  public IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences
    => AffectingTenantEntitiesIdentities.Select(x => new EntityReference<TEntityIdentity>(typeof(TEntity), x));
}
