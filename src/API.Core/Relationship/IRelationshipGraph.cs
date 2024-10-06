namespace Fossa.API.Core.Relationship;

public interface IRelationshipGraph
{
  Task<bool> HasDependentEntitiesAsync<T>(T id, CancellationToken cancellationToken) where T : IEquatable<T>;
}
