using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanyLicenseControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public CompanyLicenseControllerWithSystemLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateCompanyLicenseWithAdministratorAccessTokenAsync()
  {
    await _factory.SeedCompanyLicenseAsync("01JBV0GR968WJ25BKJVT8NDXEY.Tenant1.ADMIN1", 4, 10, default);

    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1");
    var licenseResponseModel = await client.GetFromJsonAsync<LicenseResponseModel<CompanyEntitlementsModel>>("/api/1.0/License/Company");

    licenseResponseModel.ShouldNotBeNull();
    licenseResponseModel.Terms.ShouldNotBeNull();
    licenseResponseModel.Terms.Licensor.ShouldNotBeNull();
    licenseResponseModel.Terms.Licensee.ShouldNotBeNull();
    licenseResponseModel.Entitlements.ShouldNotBeNull();
    licenseResponseModel.Entitlements.MaximumBranchCount.ShouldBe(4);
    licenseResponseModel.Entitlements.MaximumEmployeeCount.ShouldBe(10);
  }

  [Fact]
  public async Task CreateCompanyLicenseWithUserAccessTokenAsync()
  {
    Func<Task> tryToCreateCompanyLicense = () => _factory.SeedCompanyLicenseAsync("01JBV0GR968WJ25BKJVT8NDXEY.Tenant1.User1", 4, 10, default);

    await tryToCreateCompanyLicense.ShouldThrowAsync<HttpRequestException>();
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedCompaniesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task RetrieveCompanyLicenseWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/1.0/License/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }
}
