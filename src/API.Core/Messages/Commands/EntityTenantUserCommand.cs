using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public abstract record EntityTenantUserCommand<TEntity, TEntityIdentity, TTenantIdentity, TUserIdentity>(
  TTenantIdentity TenantID,
  TUserIdentity UserID)
  : ITenantUserCommand<TEntityIdentity, TTenantIdentity, TUserIdentity>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
  public abstract IEnumerable<TEntityIdentity> AffectingTenantEntitiesIdentities { get; }

  public IEnumerable<AffectingEntity<TEntityIdentity>> AffectingTenantEntities
    => AffectingTenantEntitiesIdentities.Select(x => new AffectingEntity<TEntityIdentity>(typeof(TEntity), x));
}
