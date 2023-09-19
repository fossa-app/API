using Fossa.API.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Core.Services;

public class SystemLicenseInitializer : ISystemLicenseInitializer
{
  public const string SystemLicensePath = "System";

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
      .ExistsAsync(SystemLicensePath, cancellationToken)
      .ConfigureAwait(false);

    if (!systemLicenseExists)
    {
      await CreateEmptySystemLicenseAsync(cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task CreateEmptySystemLicenseAsync(
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Creating empty System License");

    await _licenseFileRepository.UploadAsync(SystemLicensePath, Array.Empty<byte>(), cancellationToken)
      .ConfigureAwait(false);

    _logger.LogInformation("Created empty System License");
  }
}
