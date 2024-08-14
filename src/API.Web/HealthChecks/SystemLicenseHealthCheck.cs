using Fossa.API.Core.Extensions;
using Fossa.API.Core.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.MongoDb;

public class SystemLicenseHealthCheck : IHealthCheck
{
  private readonly ISystemLicenseRetriever _systemLicenseRetriever;

  public SystemLicenseHealthCheck(
    ISystemLicenseRetriever systemLicenseRetriever)
  {
    _systemLicenseRetriever = systemLicenseRetriever ?? throw new ArgumentNullException(nameof(systemLicenseRetriever));
  }

  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
  {
    try
    {
      var licenseValidation = await _systemLicenseRetriever.GetAsync(cancellationToken).ConfigureAwait(false);

      licenseValidation.GetOrThrow();

      return HealthCheckResult.Healthy();
    }
    catch (Exception ex)
    {
      return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
    }
  }
}
