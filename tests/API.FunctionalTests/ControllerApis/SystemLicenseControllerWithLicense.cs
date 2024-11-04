using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class SystemLicenseControllerWithLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly HttpClient _client;

  public SystemLicenseControllerWithLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task RetrieveSystemLicenseAsync()
  {
    var response = await _client.GetAsync("/api/1.0/License/System");
    response.EnsureSuccessStatusCode();
    var licenseResponseModel =
      await response.Content.ReadFromJsonAsync<LicenseResponseModel<SystemEntitlementsModel>>();

    licenseResponseModel.ShouldNotBeNull();
    licenseResponseModel.Terms.ShouldNotBeNull();
    licenseResponseModel.Terms.Licensor.ShouldNotBeNull();
    licenseResponseModel.Terms.Licensee.ShouldNotBeNull();
    licenseResponseModel.Entitlements.ShouldNotBeNull();
    licenseResponseModel.Entitlements.EnvironmentName.ShouldNotBeNull();
    licenseResponseModel.Entitlements.EnvironmentKind.ShouldNotBeNull();
    licenseResponseModel.Entitlements.Countries.ShouldNotBeEmpty();
    licenseResponseModel.Entitlements.MaximumCompanyCount.ShouldBePositive();
  }

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
  }

  public Task DisposeAsync() => Task.CompletedTask;
}
