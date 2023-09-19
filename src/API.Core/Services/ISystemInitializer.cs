namespace Fossa.API.Core.Services;

public interface ISystemInitializer
{
  Task InitializeAsync(CancellationToken cancellationToken);
}
