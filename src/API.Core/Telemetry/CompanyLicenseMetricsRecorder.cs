using System.Diagnostics.Metrics;
using Fossa.API.Core.Entities;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Telemetry;

public class CompanyLicenseMetricsRecorder : ICompanyLicenseMetricsRecorder
{
  private readonly Gauge<double> _daysLeft;
  private readonly TimeProvider _timeProvider;

  public CompanyLicenseMetricsRecorder(
      TimeProvider timeProvider,
      IMeterFactory meterFactory)
  {
    var meter = meterFactory.Create("Fossa.API.Core.Telemetry");
    _daysLeft = meter.CreateGauge<double>("fossa.api.core.telemetry.company_license_days_left");
    _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
  }

  public void Record(CompanyId companyId, Validation<Error, License<CompanyEntitlements>> license)
  {
    _daysLeft.Record(
        LicenseMetricsRecorderHelper.CalculateDaysLeft(license, _timeProvider),
        LicenseMetricsRecorderHelper.CreateLicenseStatusTag(license),
        new KeyValuePair<string, object?>("company_id", companyId.AsPrimitive()));
  }
}
