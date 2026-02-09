namespace Fossa.API.Core.Messages;

public interface ITenantEntityReferences<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
{
  IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences { get; }
}
