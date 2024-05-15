using System.Globalization;
using System.Net.Mail;
using EasyDoubles;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.API.FunctionalTests.Services;
using Fossa.Licensing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Deployment;
using TIKSN.Licensing;
using static LanguageExt.Prelude;

namespace Fossa.API.FunctionalTests.Seed;

public static class SystemLicenseExtensions
{
  public static async Task SeedSystemLicenseAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    var licensor = new OrganizationParty("Microsoft Corporation", "Microsoft", new MailAddress("info@microsoft.com"), new Uri("https://microsoft.com/"));
    var licensee = new OrganizationParty("Alphabet Inc.", "Google", new MailAddress("info@google.com"), new Uri("https://www.google.com"));
    var licenseTerms = new LicenseTerms(Ulid.NewUlid(), licensor, licensee, DateTimeOffset.Now.AddYears(-1), DateTimeOffset.Now.AddYears(1));
    var systemEntitlements = new SystemEntitlements(
      Ulid.NewUlid(),
      EnvironmentName.Parse("Development", asciiOnly: true, CultureInfo.InvariantCulture).Single(),
      10,
      Seq(new RegionInfo("CA"), new RegionInfo("PL"), new RegionInfo("UA"), new RegionInfo("US")));

    var testCertificateProvider = factory.Services.GetRequiredService<ITestCertificateProvider>();
    var licenseFactory = factory.Services.GetRequiredService<ILicenseFactory<SystemEntitlements, SystemLicenseEntitlements>>();
    var licenseFileRepository = factory.Services.GetRequiredService<ILicenseFileRepository>();

    var license = licenseFactory.Create(licenseTerms, systemEntitlements, testCertificateProvider.Certificate)
      .GetOrThrow();
    if (await licenseFileRepository.ExistsAsync(LicensePaths.SystemLicensePath, cancellationToken).ConfigureAwait(false))
    {
      await licenseFileRepository.DeleteAsync(LicensePaths.SystemLicensePath, cancellationToken).ConfigureAwait(false);
    }
    await licenseFileRepository.UploadAsync(LicensePaths.SystemLicensePath, [.. license.Data], cancellationToken).ConfigureAwait(false);
  }
}
