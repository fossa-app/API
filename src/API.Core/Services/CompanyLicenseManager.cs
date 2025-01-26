using Fossa.API.Core.Entities;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public abstract class CompanyLicenseManager
{
  protected readonly ICertificateProvider _certificateProvider;
  protected readonly ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements> _licenseFactory;

  protected CompanyLicenseManager(
    ICertificateProvider certificateProvider,
    ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements> licenseFactory)
  {
    _certificateProvider = certificateProvider ?? throw new ArgumentNullException(nameof(certificateProvider));
    _licenseFactory = licenseFactory ?? throw new ArgumentNullException(nameof(licenseFactory));
  }

  protected async Task<Validation<Error, License<CompanyEntitlements>>> ValidateCompanyLicenseAsync(
    License<SystemEntitlements> systemLicense,
    CompanyId companyId,
    Seq<byte> licenseData,
    CancellationToken cancellationToken)
  {
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
