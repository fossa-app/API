using System.Net;
using System.Net.Http.Headers;
using Fossa.API.Web;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanyLicenseControllerEmptyDatabase : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public CompanyLicenseControllerEmptyDatabase(
    CustomWebApplicationFactory<DefaultWebModule> factory,
    ITestOutputHelper testOutputHelper)
  {
    ArgumentNullException.ThrowIfNull(factory);

    _factory = factory;
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Fact]
  public async Task CreateMissingCompanyLicenseWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1R22PNGNDJNP12A506EFWZ.Tenant1.ADMIN1");
    using var content = new MultipartFormDataContent();
    var buffer = new byte[1024 * 1024];
    Random.Shared.NextBytes(buffer);
    var fileContent = new ByteArrayContent(buffer);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
    content.Add(fileContent, "licenseFile", "CompanyLicense.lic");

    var response = await client.PostAsync("/api/1.0/License/Company", content);

    _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateMissingCompanyLicenseWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1R22PNGNDJNP12A506EFWZ.Tenant1.User1");
    using var content = new MultipartFormDataContent();
    var buffer = new byte[1024 * 1024];
    Random.Shared.NextBytes(buffer);
    var fileContent = new ByteArrayContent(buffer);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
    content.Add(fileContent, "licenseFile", "CompanyLicense.lic");

    var response = await client.PostAsync("/api/1.0/License/Company", content);
    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task RetrieveCompanyLicenseWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1R22PNGNDJNP12A506EFWZ.Tenant1.User1");
    var response = await client.GetAsync("/api/1.0/License/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task RetrieveCompanyLicenseWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/1.0/License/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }
}
