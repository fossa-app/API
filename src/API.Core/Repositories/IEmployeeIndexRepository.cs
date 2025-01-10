namespace Fossa.API.Core.Repositories;

public interface IEmployeeIndexRepository
{
  Task EnsureIndexesCreatedAsync(
    CancellationToken cancellationToken);
}
