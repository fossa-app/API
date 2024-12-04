using System.Globalization;
using Fossa.API.Core.Services;

namespace Fossa.API.Core.Validators;

public static class CompanyCommandValidatorHelper
{
  public static async Task<bool> CompanyCountryMustBeLicensedAsync(
    ISystemLicenseRetriever systemLicenseRetriever,
    RegionInfo companyCountry,
    CancellationToken cancellationToken)
  {
    var systemLicenseResult = await systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);

    return systemLicenseResult.Match(license => license.Entitlements.Countries.Contains(companyCountry), _ => false);
  }
}
