using Fossa.API.Core.Messages;
using Fossa.API.Core.Relationship;

namespace Fossa.API.Core.Extensions;

public static class RelationshipGraphExtensions
{
  public static async Task ThrowIfHasDependentEntitiesAsync<T>(this IRelationshipGraph relationshipGraph, T id, CancellationToken cancellationToken) where T : IEquatable<T>
  {
    ArgumentNullException.ThrowIfNull(relationshipGraph);

    var hasDependentEntities = await relationshipGraph.HasDependentEntitiesAsync(id, cancellationToken).ConfigureAwait(false);

    if (hasDependentEntities)
    {
      throw new FailedDependencyException($"Entity with {id.GetType().Name}({id}) has dependent entities.");
    }
  }
}
