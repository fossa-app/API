namespace Fossa.API.Core.Services;

public interface ISystemLicenseInitializer
{
  Task InitializeAsync(CancellationToken cancellationToken);
}
