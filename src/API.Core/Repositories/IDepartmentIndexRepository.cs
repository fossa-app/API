namespace Fossa.API.Core.Repositories;

public interface IDepartmentIndexRepository
{
  Task EnsureIndexesCreatedAsync(
    CancellationToken cancellationToken);
}
