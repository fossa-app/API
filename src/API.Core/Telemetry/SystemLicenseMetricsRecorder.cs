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
      LicenseMetricsRecorderHelper.CalculateDaysLeft(license, _timeProvider),
      LicenseMetricsRecorderHelper.CreateLicenseStatusTag(license));
  }
}
