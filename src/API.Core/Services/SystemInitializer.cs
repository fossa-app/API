using Fossa.API.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Core.Services;

public class SystemInitializer : ISystemInitializer
{
  private readonly ISystemPropertiesInitializer _systemPropertiesInitializer;
  private readonly ISystemLicenseInitializer _systemLicenseInitializer;
  private readonly IBranchIndexRepository _branchIndexRepository;
  private readonly IEmployeeIndexRepository _employeeIndexRepository;
  private readonly ILogger<SystemInitializer> _logger;

  public SystemInitializer(
    ISystemPropertiesInitializer systemPropertiesInitializer,
    ISystemLicenseInitializer systemLicenseInitializer,
    IBranchIndexRepository branchIndexRepository,
    IEmployeeIndexRepository employeeIndexRepository,
    ILogger<SystemInitializer> logger)
  {
    _systemPropertiesInitializer = systemPropertiesInitializer ?? throw new ArgumentNullException(nameof(systemPropertiesInitializer));
    _systemLicenseInitializer = systemLicenseInitializer ?? throw new ArgumentNullException(nameof(systemLicenseInitializer));
    _branchIndexRepository = branchIndexRepository ?? throw new ArgumentNullException(nameof(branchIndexRepository));
    _employeeIndexRepository = employeeIndexRepository ?? throw new ArgumentNullException(nameof(employeeIndexRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task InitializeAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation(2057718973, "System Initialization Started");
    await _systemPropertiesInitializer.InitializeAsync(cancellationToken).ConfigureAwait(false);
    await _systemLicenseInitializer.InitializeAsync(cancellationToken).ConfigureAwait(false);
    _logger.LogInformation(2057718973, "System Initialization Finished");

    await _branchIndexRepository.EnsureIndexesCreatedAsync(cancellationToken).ConfigureAwait(false);
    await _employeeIndexRepository.EnsureIndexesCreatedAsync(cancellationToken).ConfigureAwait(false);
  }
}
