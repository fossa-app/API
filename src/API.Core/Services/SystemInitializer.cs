using Microsoft.Extensions.Logging;

namespace Fossa.API.Core.Services;

public class SystemInitializer : ISystemInitializer
{
  private readonly ISystemPropertiesInitializer _systemPropertiesInitializer;
  private readonly ILogger<SystemInitializer> _logger;

  public SystemInitializer(
    ISystemPropertiesInitializer systemPropertiesInitializer,
    ILogger<SystemInitializer> logger)
  {
    _systemPropertiesInitializer = systemPropertiesInitializer ??
                                   throw new ArgumentNullException(nameof(systemPropertiesInitializer));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task InitializeAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation(2057718973, "System Initialization Started");
    await _systemPropertiesInitializer.InitializeAsync(cancellationToken).ConfigureAwait(false);
    _logger.LogInformation(2057718973, "System Initialization Finished");
  }
}
