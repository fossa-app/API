using System.Net.Http.Json;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.FunctionalTests.ControllerApis;

[Collection("Sequential")]
public class LicenseController : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly HttpClient _client;

  public LicenseController(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
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
}
