using Fossa.API.Core.Services;
using TIKSN.Globalization;

namespace Fossa.API.Core.Validators;

public static class CompanyCommandValidatorHelper
{
  public static async Task<bool> CompanyCountryMustBeLicensedAsync(
    ISystemLicenseRetriever systemLicenseRetriever,
    CountryInfo companyCountry,
    CancellationToken cancellationToken)
  {
    var systemLicenseResult = await systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);

    return systemLicenseResult.Match(license => license.Entitlements.Countries.Contains(companyCountry), _ => false);
  }

  public static string CompanyCountryMustBeLicensedErrorMessage<T>(T command, CountryInfo property)
  {
    return $"Country '{property} - {property.PrincipalRegion.EnglishName}' is not licensed.";
  }
}
