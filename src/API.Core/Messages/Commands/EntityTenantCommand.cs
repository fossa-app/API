using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public abstract record EntityTenantCommand<TEntity, TEntityIdentity, TTenantIdentity>(
  TTenantIdentity TenantID)
  : ITenantCommand<TEntityIdentity, TTenantIdentity>, ITenantEntityReferences<TEntityIdentity>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  public abstract IEnumerable<TEntityIdentity> AffectingTenantEntitiesIdentities { get; }

  public IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences
    => AffectingTenantEntitiesIdentities.Select(x => new EntityReference<TEntityIdentity>(typeof(TEntity), x));
}
