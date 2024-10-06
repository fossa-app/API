namespace Fossa.API.Core.Repositories;

public interface IDependencyQueryRepository<T>
  where T : IEquatable<T>
{
  Task<bool> HasDependencyAsync(
    T id,
    CancellationToken cancellationToken);
}
