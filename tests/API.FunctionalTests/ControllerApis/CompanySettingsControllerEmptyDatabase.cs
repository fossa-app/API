using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanySettingsControllerEmptyDatabase : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly HttpClient _client;

  public CompanySettingsControllerEmptyDatabase(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task CreateCompanySettingsWithoutAccessTokenAsync()
  {
    var response = await _client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("test-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateCompanySettingsWithUserAccessTokenAsync()
  {
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");

    var response = await _client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("test-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task GetCompanySettingsWithoutAccessTokenAsync()
  {
    var response = await _client.GetAsync("/api/1.0/CompanySettings");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task GetCompanySettingsWithAccessTokenButNoCompanyAsync()
  {
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant1000.User1");

    var response = await _client.GetAsync("/api/1.0/CompanySettings");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UpdateCompanySettingsWithoutAccessTokenAsync()
  {
    var response = await _client.PutAsJsonAsync("/api/1.0/CompanySettings/1",
      new CompanySettingsModificationModel("updated-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task UpdateCompanySettingsWithUserAccessTokenAsync()
  {
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");

    var response = await _client.PutAsJsonAsync("/api/1.0/CompanySettings/1",
      new CompanySettingsModificationModel("updated-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteCompanySettingsWithoutAccessTokenAsync()
  {
    var response = await _client.DeleteAsync("/api/1.0/CompanySettings/1");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteCompanySettingsWithUserAccessTokenAsync()
  {
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");

    var response = await _client.DeleteAsync("/api/1.0/CompanySettings/1");

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }
}
