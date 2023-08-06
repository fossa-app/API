namespace Fossa.API.Core.Messages;

public record AffectingEntity<TEntityIdentity>(
  Type EntityType,
  TEntityIdentity EntityID)
  where TEntityIdentity : IEquatable<TEntityIdentity>;
