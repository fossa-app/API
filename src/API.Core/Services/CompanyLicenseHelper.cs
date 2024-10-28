using System.Globalization;
using Fossa.API.Core.Entities;

namespace Fossa.API.Core.Services;

public static class CompanyLicenseHelper
{
  public static string GetCompanyLicensePath(CompanyId companyId)
  {
    return string.Format(CultureInfo.InvariantCulture, LicensePaths.CompanyLicensePathFormat, companyId.AsPrimitive());
  }
}
