using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using NodaTime;

namespace Fossa.API.Core.Validators;

public static class BranchCommandValidatorHelper
{
  public static async Task<bool> BranchTimeZoneCountryMustBeCompanyCountryAsync(
    ICompanyQueryRepository companyQueryRepository,
    IDateTimeZoneLookup dateTimeZoneLookup,
    Guid tenantId,
    DateTimeZone branchTimeZone,
    CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(companyQueryRepository);
    ArgumentNullException.ThrowIfNull(dateTimeZoneLookup);
    ArgumentNullException.ThrowIfNull(branchTimeZone);

    var companyEntity = await companyQueryRepository.GetByTenantIdAsync(tenantId, cancellationToken).ConfigureAwait(false);
    return string.Equals(
      companyEntity.Country.TwoLetterISORegionName,
      dateTimeZoneLookup.ResolveTimeZoneRegion(branchTimeZone).TwoLetterISORegionName,
      StringComparison.OrdinalIgnoreCase);
  }

  public static string BranchTimeZoneCountryMustBeCompanyCountryErrorMessage<T>(T command, DateTimeZone property)
  {
    return $"Time Zone '{property.Id}' is not for Company Country.";
  }
}
