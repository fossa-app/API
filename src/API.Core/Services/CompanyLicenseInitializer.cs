using System.Globalization;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Core.Services;

public class CompanyLicenseInitializer : ICompanyLicenseInitializer
{
  private readonly ILicenseFileRepository _licenseFileRepository;
  private readonly ILogger<CompanyLicenseInitializer> _logger;

  public CompanyLicenseInitializer(
    ILicenseFileRepository licenseFileRepository,
    ILogger<CompanyLicenseInitializer> logger)
  {
    _licenseFileRepository = licenseFileRepository ?? throw new ArgumentNullException(nameof(licenseFileRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task InitializeAsync(CompanyId companyId, CancellationToken cancellationToken)
  {
    _logger.LogInformation(24705152, "Checking existence of Company License");
    var companyLicensePath = string.Format(CultureInfo.InvariantCulture, LicensePaths.CompanyLicensePathFormat, companyId.AsPrimitive());
    var companyLicenseExists = await _licenseFileRepository
      .ExistsAsync(companyLicensePath, cancellationToken)
      .ConfigureAwait(false);

    if (!companyLicenseExists)
    {
      await CreateEmptyCompanyLicenseAsync(companyLicensePath, cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task CreateEmptyCompanyLicenseAsync(
    string companyLicensePath,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation(24705164, "Creating empty Company License");

    await _licenseFileRepository.UploadAsync(companyLicensePath, [], cancellationToken)
      .ConfigureAwait(false);

    _logger.LogInformation(24705169, "Created empty Company License");
  }
}
