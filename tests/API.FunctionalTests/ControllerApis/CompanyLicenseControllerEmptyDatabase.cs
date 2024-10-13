using System.Net;
using System.Net.Http.Headers;
using Fossa.API.Web;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanyLicenseControllerEmptyDatabase : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public CompanyLicenseControllerEmptyDatabase(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    ArgumentNullException.ThrowIfNull(factory);

    _factory = factory;
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
