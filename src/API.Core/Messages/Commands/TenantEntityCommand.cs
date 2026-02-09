using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public abstract record TenantEntityCommand<TEntity, TEntityIdentity, TTenantIdentity>(
  TTenantIdentity TenantID)
  : ITenantEntityCommand<TEntityIdentity, TTenantIdentity>, ITenantEntityReferences<TEntityIdentity>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  public abstract IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }

  public IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences
    => TenantEntityIdentities.Select(x => new EntityReference<TEntityIdentity>(typeof(TEntity), x));
}
