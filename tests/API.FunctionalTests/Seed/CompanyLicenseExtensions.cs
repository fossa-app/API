using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Services;
using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.FunctionalTests.Services;
using Fossa.API.Web.ApiModels;
using Fossa.Licensing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Licensing;

namespace Fossa.API.FunctionalTests.Seed;

public static class CompanyLicenseExtensions
{
  public static async Task SeedCompanyLicensesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await SeedCompanyLicenseAsync(factory, "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1", 300, 200, 100, cancellationToken).ConfigureAwait(false);
    await SeedCompanyLicenseAsync(factory, "01JB0RAH24ZJBA53AJF5F5MMZX.Tenant2.ADMIN1", 300, 10, 5, cancellationToken).ConfigureAwait(false);
    await SeedCompanyLicenseAsync(factory, "01JB0RAH24ZJBA53AJF5F5MMZX.Tenant3.ADMIN1", 300, 10, 5, cancellationToken).ConfigureAwait(false);
  }

  public static async Task SeedCompanyLicenseAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    string accessToken,
    int maximumEmployeeCount,
    int maximumBranchCount,
    int maximumDepartmentCount,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    var testCertificateProvider = factory.Services.GetRequiredService<ITestCertificateProvider>();
    var licenseFactory = factory.Services.GetRequiredService<ILicenseFactory<CompanyEntitlements, CompanyLicenseEntitlements>>();
    var systemPropertiesRepository = factory.Services.GetRequiredService<SystemPropertiesMongoEasyRepository>();

    var systemPropertiesEntity = await systemPropertiesRepository.GetAsync(SystemProperties.MainSystemPropertiesId.AsPrimitive(), default).ConfigureAwait(false);

    var client = factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var companyRetrievalModel = await client.GetFromJsonAsync<CompanyRetrievalModel>("/api/1.0/Company", cancellationToken).ConfigureAwait(false)
      ?? throw new InvalidOperationException("Company is not created yet.");

    var companyLicenseRetrievalResponse = await client.GetAsync("/api/1.0/License/Company", cancellationToken).ConfigureAwait(false);

    if (!companyLicenseRetrievalResponse.IsSuccessStatusCode)
    {
      var licensor = new OrganizationParty("Microsoft Corporation", "Microsoft", new MailAddress("info@microsoft.com"),
        new Uri("https://microsoft.com/"));
      var licensee = new OrganizationParty($"Alphabet {companyRetrievalModel.Id} Inc.",
        $"Google {companyRetrievalModel.Id}", new MailAddress("info@google.com"), new Uri("https://www.google.com"));
      var licenseTerms = new LicenseTerms(Ulid.NewUlid(), licensor, licensee, DateTimeOffset.Now.AddYears(-1),
        DateTimeOffset.Now.AddYears(1));
      var companyEntitlements = new CompanyEntitlements(
        new Ulid(systemPropertiesEntity.SystemID),
        companyRetrievalModel.Id,
        maximumBranchCount,
        maximumEmployeeCount,
        maximumDepartmentCount);

      var license = licenseFactory.Create(licenseTerms, companyEntitlements, testCertificateProvider.Certificate)
        .GetOrThrow();

      using var content = new MultipartFormDataContent();
      var fileContent = new ByteArrayContent([.. license.Data]);
      fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
      content.Add(fileContent, "licenseFile", "CompanyLicense.lic");

      var response = await client.PostAsync("/api/1.0/License/Company", content, cancellationToken)
        .ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
    }
  }
}
