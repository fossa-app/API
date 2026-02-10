using TIKSN.Data;
using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Core.Messages.Queries;


public abstract record TenantEntityQuery<TEntity, TEntityIdentity, TTenantIdentity, TResult>(
  TTenantIdentity TenantID)
  : ITenantEntityQuery<TEntityIdentity, TTenantIdentity, TResult>, ITenantEntityReferences<TEntityIdentity>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  public abstract IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }

  public IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences
    => TenantEntityIdentities.Select(x => new EntityReference<TEntityIdentity>(typeof(TEntity), x));
}

