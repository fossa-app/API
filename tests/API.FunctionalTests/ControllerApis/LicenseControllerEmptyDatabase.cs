using System.Net;
using Fossa.API.Web;

namespace Fossa.API.FunctionalTests.ControllerApis;

[Collection("Sequential")]
public class LicenseControllerEmptyDatabase : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly HttpClient _client;

  public LicenseControllerEmptyDatabase(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task RetrieveSystemLicenseAsync()
  {
    var response = await _client.GetAsync("/api/1.0/License/System");

    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
  }
}
