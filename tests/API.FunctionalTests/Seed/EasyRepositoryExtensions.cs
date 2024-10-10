using EasyDoubles;
using TIKSN.Data;

namespace Fossa.API.FunctionalTests.Seed;

public static class EasyRepositoryExtensions
{
  public static async Task TryAddAsync<TEntity, TIdentity>(
    this IEasyRepository<TEntity, TIdentity> easyRepository,
    TEntity entity,
    CancellationToken cancellationToken)
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
  {
    if (!await easyRepository.ExistsAsync(entity.ID, cancellationToken).ConfigureAwait(false))
    {
      await easyRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }
  }
}
