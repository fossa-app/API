using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Services;

public class CompanyLicenseRetriever : CompanyLicenseManager, ICompanyLicenseRetriever
{
  private readonly ILicenseFileRepository _licenseFileRepository;
  private readonly ISystemLicenseRetriever _systemLicenseRetriever;

  public CompanyLicenseRetriever(
    ILicenseFileRepository licenseFileRepository,
    ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements> licenseFactory,
    ICertificateProvider certificateProvider,
    ISystemLicenseRetriever systemLicenseRetriever)
    : base(certificateProvider, licenseFactory)
  {
    _licenseFileRepository = licenseFileRepository ?? throw new ArgumentNullException(nameof(licenseFileRepository));
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
    var companyLicensePath = CompanyLicenseHelper.GetCompanyLicensePath(companyId);
    var licenseFile = await _licenseFileRepository.DownloadAsync(companyLicensePath, cancellationToken)
      .ConfigureAwait(false);

    var licenseData = licenseFile.Content.ToSeq();

    return await ValidateCompanyLicenseAsync(
      systemLicense,
      companyId,
      licenseData,
      cancellationToken)
      .ConfigureAwait(false);
  }
}
