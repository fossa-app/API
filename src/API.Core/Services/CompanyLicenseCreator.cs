using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public class CompanyLicenseCreator : CompanyLicenseManager, ICompanyLicenseCreator
{
  private readonly ILicenseFileRepository _licenseFileRepository;
  private readonly ISystemLicenseRetriever _systemLicenseRetriever;

  public CompanyLicenseCreator(
    ILicenseFileRepository licenseFileRepository,
    ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements> licenseFactory,
    ICertificateProvider certificateProvider,
    ISystemLicenseRetriever systemLicenseRetriever)
    : base(certificateProvider, licenseFactory)
  {
    _licenseFileRepository = licenseFileRepository ?? throw new ArgumentNullException(nameof(licenseFileRepository));
    _systemLicenseRetriever = systemLicenseRetriever ?? throw new ArgumentNullException(nameof(systemLicenseRetriever));
  }

  public async Task<Validation<Error, License<CompanyEntitlements>>> CreateAsync(
    CompanyId companyId,
    Seq<byte> licenseData,
    CancellationToken cancellationToken)
  {
    var systemLicense = await _systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);
    var potencialLicense = await systemLicense.MatchAsync(
      x => ValidateCompanyLicenseAsync(x, companyId, licenseData, cancellationToken),
      _ => Fail<Error, License<CompanyEntitlements>>(Error.New(24709695, "System License is invalid.")))
      .ConfigureAwait(false);

    return await potencialLicense.MatchAsync(
      v => CreateCompanyLicenseAsync(v, companyId, licenseData, cancellationToken),
      f => f).ConfigureAwait(false);
  }

  private async Task<Validation<Error, License<CompanyEntitlements>>> CreateCompanyLicenseAsync(
    License<CompanyEntitlements> companyLicense,
    CompanyId companyId,
    Seq<byte> licenseData,
    CancellationToken cancellationToken)
  {
    var companyLicensePath = CompanyLicenseHelper.GetCompanyLicensePath(companyId);
    var companyLicenseExists = await _licenseFileRepository.ExistsAsync(companyLicensePath, cancellationToken).ConfigureAwait(false);

    if (companyLicenseExists)
    {
      var companyLicenseFile = await _licenseFileRepository.DownloadAsync(companyLicensePath, cancellationToken).ConfigureAwait(false);
      if (companyLicenseFile?.Content.Count > 0)
      {
        return Fail<Error, License<CompanyEntitlements>>(Error.New(25578518, "Company License already exists."));
      }

      if (companyLicenseFile?.Content.Count == 0)
      {
        await _licenseFileRepository.DeleteAsync(companyLicensePath, cancellationToken).ConfigureAwait(false);
      }
    }

    await _licenseFileRepository.UploadAsync(companyLicensePath, [.. licenseData], cancellationToken).ConfigureAwait(false);

    return companyLicense;
  }
}
