using System.Text;

namespace Fossa.API.Core.Services;

public static class LicensePaths
{
  public const string SystemLicensePath = "System";
  public static readonly CompositeFormat CompanyLicensePathFormat = CompositeFormat.Parse("Company{0}");
}
