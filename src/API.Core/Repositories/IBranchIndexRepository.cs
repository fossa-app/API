namespace Fossa.API.Core.Repositories;

public interface IBranchIndexRepository
{
  Task EnsureIndexesCreatedAsync(
    CancellationToken cancellationToken);
}
