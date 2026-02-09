using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public abstract record TenantUserEntityCommand<TEntity, TEntityIdentity, TTenantIdentity, TUserIdentity>(
  TTenantIdentity TenantID,
  TUserIdentity UserID)
  : ITenantUserEntityCommand<TEntityIdentity, TTenantIdentity, TUserIdentity>, ITenantEntityReferences<TEntityIdentity>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
  public abstract IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }

  public IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences
    => TenantEntityIdentities.Select(x => new EntityReference<TEntityIdentity>(typeof(TEntity), x));
}
