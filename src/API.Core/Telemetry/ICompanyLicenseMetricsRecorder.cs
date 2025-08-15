using Fossa.API.Core.Entities;
using Fossa.Licensing;
using TIKSN.Licensing;

namespace Fossa.API.Core.Telemetry;

public interface ICompanyLicenseMetricsRecorder
{
  void Record(CompanyId companyId, Validation<Error, License<CompanyEntitlements>> license);
}
