namespace Fossa.API.Core.Services;

public interface ISystemPropertiesInitializer
{
  Task InitializeAsync(CancellationToken cancellationToken);
}
