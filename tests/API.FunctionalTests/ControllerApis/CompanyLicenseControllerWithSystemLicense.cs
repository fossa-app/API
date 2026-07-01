using System.Net;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
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
    await _factory.SeedCompanyLicenseAsync("01JBV0GR968WJ25BKJVT8NDXEY.Tenant1.ADMIN1", 10, 4, 2, TestContext.Current.CancellationToken);

    using var scope = _factory.Services.CreateScope();
    var companyLicenseClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyLicenseClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1");
    var licenseResponseModel = (await companyLicenseClient.getLicenseAsync(TestContext.Current.CancellationToken)).Unwrap();

    licenseResponseModel.ShouldNotBeNull();
    licenseResponseModel.terms.ShouldNotBeNull();
    licenseResponseModel.terms.licensor.ShouldNotBeNull();
    licenseResponseModel.terms.licensee.ShouldNotBeNull();
    licenseResponseModel.entitlements.ShouldNotBeNull();
    licenseResponseModel.entitlements.maximumBranchCount.ShouldBe(4);
    licenseResponseModel.entitlements.maximumEmployeeCount.ShouldBe(10);
  }

  [Fact]
  public async Task CreateCompanyLicenseWithUserAccessTokenAsync()
  {
    Func<Task> tryToCreateCompanyLicense = () => _factory.SeedCompanyLicenseAsync("01JBV0GR968WJ25BKJVT8NDXEY.Tenant2.User1", 4, 10, 2, TestContext.Current.CancellationToken);

    await tryToCreateCompanyLicense.ShouldThrowAsync<HttpRequestException>();
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
    await _factory.SeedCompaniesAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  [Fact]
  public async Task RetrieveCompanyLicenseWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyLicenseClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyLicenseClient;

    (await companyLicenseClient.getLicenseAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }
}
