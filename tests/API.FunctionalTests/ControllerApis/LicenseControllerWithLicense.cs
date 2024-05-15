using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.FunctionalTests.ControllerApis;

[Collection("Sequential")]
public class LicenseControllerWithLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly HttpClient _client;

  public LicenseControllerWithLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
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

    Assert.NotNull(licenseResponseModel);
    Assert.NotNull(licenseResponseModel.Terms);
    Assert.NotNull(licenseResponseModel.Terms.Licensor);
    Assert.NotNull(licenseResponseModel.Terms.Licensee);
    Assert.NotNull(licenseResponseModel.Entitlements);
    Assert.NotNull(licenseResponseModel.Entitlements.EnvironmentName);
    Assert.NotNull(licenseResponseModel.Entitlements.EnvironmentKind);
    Assert.True(licenseResponseModel.Entitlements.MaximumCompanyCount > 0);
  }

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
  }

  public Task DisposeAsync() => Task.CompletedTask;
}
