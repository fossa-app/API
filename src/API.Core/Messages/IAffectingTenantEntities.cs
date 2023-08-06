namespace Fossa.API.Core.Messages;

public interface IAffectingTenantEntities<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
{
  IEnumerable<AffectingEntity<TEntityIdentity>> AffectingTenantEntities { get; }
}
