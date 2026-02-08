namespace Fossa.API.Core.Messages;

public record EntityReference<TEntityIdentity>(
  Type EntityType,
  TEntityIdentity EntityID)
  where TEntityIdentity : IEquatable<TEntityIdentity>;
