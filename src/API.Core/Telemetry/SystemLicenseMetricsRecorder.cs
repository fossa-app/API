using System.Diagnostics.Metrics;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Telemetry;

public class SystemLicenseMetricsRecorder : ISystemLicenseMetricsRecorder
{
  private readonly Gauge<double> _daysLeft;
  private readonly TimeProvider _timeProvider;

  public SystemLicenseMetricsRecorder(
    TimeProvider timeProvider,
    IMeterFactory meterFactory)
  {
    var meter = meterFactory.Create("Fossa.API.Core.Telemetry");
    _daysLeft = meter.CreateGauge<double>("fossa.api.core.telemetry.system_license_days_left");
    _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
  }

  public void Record(Validation<Error, License<SystemEntitlements>> license)
  {
    _daysLeft.Record(
      license.Match(
        validLicense => CalculateTimeLeft(validLicense).TotalDays,
        _ => 0.0),
      new KeyValuePair<string, object?>("license_status",
        license.Match(
          validLicense => "valid",
          _ => "invalid")));
  }

  private TimeSpan CalculateTimeLeft(License<SystemEntitlements> validLicense)
  {
    var now = _timeProvider.GetUtcNow();
    if (validLicense.Terms.NotAfter <= now)
    {
      return TimeSpan.Zero;
    }

    return validLicense.Terms.NotAfter.Subtract(now);
  }
}
