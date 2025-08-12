using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Telemetry;

public interface ISystemLicenseMetricsRecorder
{
  /// <summary>
  /// Records the system license metrics.
  /// </summary>
  /// <param name="license">The license to record.</param>
  void Record(Validation<Error, License<SystemEntitlements>> license);
}
