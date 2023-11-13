using Fossa.API.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Core.Services;

public class SystemLicenseInitializer : ISystemLicenseInitializer
{
  private readonly ILicenseFileRepository _licenseFileRepository;
  private readonly ILogger<SystemLicenseInitializer> _logger;

  public SystemLicenseInitializer(
    ILicenseFileRepository licenseFileRepository,
    ILogger<SystemLicenseInitializer> logger)
  {
    _licenseFileRepository = licenseFileRepository ?? throw new ArgumentNullException(nameof(licenseFileRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task InitializeAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation(1333693557, "Checking existence of System License");
    var systemLicenseExists = await _licenseFileRepository
      .ExistsAsync(LicensePaths.SystemLicensePath, cancellationToken)
      .ConfigureAwait(false);

    if (!systemLicenseExists)
    {
      await CreateEmptySystemLicenseAsync(cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task CreateEmptySystemLicenseAsync(
    CancellationToken cancellationToken)
  {
    _logger.LogInformation(1972365262, "Creating empty System License");

    await _licenseFileRepository.UploadAsync(LicensePaths.SystemLicensePath, Array.Empty<byte>(), cancellationToken)
      .ConfigureAwait(false);

    _logger.LogInformation(1474946684, "Created empty System License");
  }
}
