using Fossa.API.Core.Repositories;
using Fossa.Licensing;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public class SystemLicenseRetriever : ISystemLicenseRetriever
{
  private readonly ICertificateProvider _certificateProvider;
  private readonly ISystemPropertiesQueryRepository _systemPropertiesQueryRepository;
  private readonly ILicenseFactory<SystemEntitlements, SystemLicenseEntitlements> _licenseFactory;
  private readonly ILicenseFileRepository _licenseFileRepository;

  public SystemLicenseRetriever(
    ILicenseFileRepository licenseFileRepository,
    ILicenseFactory<SystemEntitlements, SystemLicenseEntitlements> licenseFactory,
    ICertificateProvider certificateProvider,
    ISystemPropertiesQueryRepository systemPropertiesQueryRepository)
  {
    _licenseFileRepository = licenseFileRepository ?? throw new ArgumentNullException(nameof(licenseFileRepository));
    _licenseFactory = licenseFactory ?? throw new ArgumentNullException(nameof(licenseFactory));
    _certificateProvider = certificateProvider ?? throw new ArgumentNullException(nameof(certificateProvider));
    _systemPropertiesQueryRepository = systemPropertiesQueryRepository ?? throw new ArgumentNullException(nameof(systemPropertiesQueryRepository));
  }

  public async Task<Validation<Error, License<SystemEntitlements>>> GetAsync(CancellationToken cancellationToken)
  {
    var licenseFile = await _licenseFileRepository.DownloadAsync(LicensePaths.SystemLicensePath, cancellationToken)
      .ConfigureAwait(false);

    var systemPropertiesEntity = await _systemPropertiesQueryRepository.GetAsync(SystemProperties.MainSystemPropertiesId, cancellationToken).ConfigureAwait(false);

    var licenseData = licenseFile.Content.ToSeq();

    var certificate = await _certificateProvider.GetCertificateAsync(cancellationToken).ConfigureAwait(false);

    var licenseValidation = _licenseFactory.Create(licenseData, certificate);

    return licenseValidation
      .Validate(
        license => license.Entitlements.SystemId == systemPropertiesEntity.SystemID,
        11751858, "Current System License is issued to another system.");
  }
}
