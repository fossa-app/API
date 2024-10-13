using System.Globalization;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.Licensing;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Licensing;
using static LanguageExt.Prelude;

namespace Fossa.API.Core.Services;

public class CompanyLicenseRetriever : ICompanyLicenseRetriever
{
  private readonly ICertificateProvider _certificateProvider;
  private readonly ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements> _licenseFactory;
  private readonly ILicenseFileRepository _licenseFileRepository;
  private readonly ISystemLicenseRetriever _systemLicenseRetriever;

  public CompanyLicenseRetriever(
    ILicenseFileRepository licenseFileRepository,
    ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements> licenseFactory,
    ICertificateProvider certificateProvider,
    ISystemLicenseRetriever systemLicenseRetriever)
  {
    _licenseFileRepository = licenseFileRepository ?? throw new ArgumentNullException(nameof(licenseFileRepository));
    _licenseFactory = licenseFactory ?? throw new ArgumentNullException(nameof(licenseFactory));
    _certificateProvider = certificateProvider ?? throw new ArgumentNullException(nameof(certificateProvider));
    _systemLicenseRetriever = systemLicenseRetriever ?? throw new ArgumentNullException(nameof(systemLicenseRetriever));
  }

  public async Task<Validation<Error, License<CompanyEntitlements>>> GetAsync(CompanyId companyId, CancellationToken cancellationToken)
  {
    var systemLicense = await _systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);
    return await systemLicense.MatchAsync(
      x => GetCompanyLicenseAsync(x, companyId, cancellationToken),
      _ => Fail<Error, License<CompanyEntitlements>>(Error.New(24709695, "System License is invalid.")))
      .ConfigureAwait(false);
  }

  private async Task<Validation<Error, License<CompanyEntitlements>>> GetCompanyLicenseAsync(
    License<SystemEntitlements> systemLicense,
    CompanyId companyId,
    CancellationToken cancellationToken)
  {
    var companyLicensePath = string.Format(CultureInfo.InvariantCulture, LicensePaths.CompanyLicensePathFormat, companyId.AsPrimitive());
    var licenseFile = await _licenseFileRepository.DownloadAsync(companyLicensePath, cancellationToken)
      .ConfigureAwait(false);

    var licenseData = licenseFile.Content.ToSeq();

    var certificate = await _certificateProvider.GetCertificateAsync(cancellationToken).ConfigureAwait(false);

    var licenseValidation = _licenseFactory.Create(licenseData, certificate);

    return licenseValidation
      .Validate(
        license => license.Entitlements.CompanyId == companyId.AsPrimitive(),
        24709381, "Current Company License is issued to another company.")
      .Validate(
        license => license.Entitlements.SystemId == systemLicense.Entitlements.SystemId,
        24709386, "Current Company License is issued for another system.");
  }
}
