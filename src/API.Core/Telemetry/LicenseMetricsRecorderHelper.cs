using TIKSN.Licensing;

namespace Fossa.API.Core.Telemetry;

internal static class LicenseMetricsRecorderHelper
{
  internal static double CalculateDaysLeft<T>(
    Validation<Error, License<T>> license,
    TimeProvider timeProvider)
  {
    return license.Match(
      validLicense => CalculateTimeLeft(validLicense, timeProvider).TotalDays,
      _ => 0.0);
  }

  internal static TimeSpan CalculateTimeLeft<T>(
    License<T> validLicense,
    TimeProvider timeProvider)
  {
    var now = timeProvider.GetUtcNow();
    if (validLicense.Terms.NotAfter <= now)
    {
      return TimeSpan.Zero;
    }

    return validLicense.Terms.NotAfter.Subtract(now);
  }

  internal static KeyValuePair<string, object?> CreateLicenseStatusTag<T>(
    Validation<Error, License<T>> license)
  {
    return new KeyValuePair<string, object?>("license_status",
      license.Match(
       validLicense => "valid",
       _ => "invalid"));
  }
}
