using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanySettingsControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public CompanySettingsControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory,
    ITestOutputHelper testOutputHelper)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Fact]
  public async Task CreateCompanySettingsWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant3.ADMIN1");

    const string colorSchemeId = "new-theme";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(colorSchemeId));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/CompanySettings");
    retrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var responseModel = await retrievalResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.CompanyId.ShouldBePositive();
    responseModel.ColorSchemeId.ShouldBe(colorSchemeId);
  }

  [Fact]
  public async Task CreateCompanySettingsWithInvalidColorSchemeIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant4.ADMIN1");

    // Test various invalid colorSchemeId formats
    var invalidColorSchemeIds = new[]
    {
      "ab", // Too short (less than 3 characters)
      "ABC", // Contains uppercase letters
      "test123", // Contains numbers
      "test--theme", // Multiple consecutive hyphens
      "-test", // Starts with hyphen
      "test-", // Ends with hyphen
      "test theme", // Contains space
      "test_theme", // Contains underscore
    };

    foreach (var invalidColorSchemeId in invalidColorSchemeIds)
    {
      var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
        new CompanySettingsModificationModel(invalidColorSchemeId));

      response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity,
        $"ColorSchemeId '{invalidColorSchemeId}' should be invalid");
    }
  }

  [Fact]
  public async Task CreateCompanySettingsWithValidColorSchemeIdFormatsAsync()
  {
    var client = _factory.CreateClient();

    // Test various valid colorSchemeId formats
    var validTestCases = new[]
    {
      new { Tenant = "Tenant45442385", ColorSchemeId = "abc" }, // Minimum length
      new { Tenant = "Tenant45442386", ColorSchemeId = "theme" }, // Simple lowercase
      new { Tenant = "Tenant45442387", ColorSchemeId = "dark-theme" }, // With hyphen in middle
      new { Tenant = "Tenant45442388", ColorSchemeId = "verylongthemename" }, // Long name
    };

    foreach (var testCase in validTestCases)
    {
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"01JA1ZJAWF27S0J8Z2VJE7673Y.{testCase.Tenant}.ADMIN1");

      const string companyName = "Company-1412593541";

      _ = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "US"));

      var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
        new CompanySettingsModificationModel(testCase.ColorSchemeId));

      if (response.StatusCode != HttpStatusCode.OK)
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Failed for ColorSchemeId '{testCase.ColorSchemeId}': {errorContent}");
      }

      response.StatusCode.ShouldBe(HttpStatusCode.OK,
        $"ColorSchemeId '{testCase.ColorSchemeId}' should be valid");
    }
  }

  [Fact]
  public async Task CreateDuplicateCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");

    // Try to create company settings for a company that already has settings
    var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("duplicate-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
  }

  [Fact]
  public async Task DeleteExistingCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0RAH24ZJBA53AJF5F5MMZX.Tenant2.ADMIN1");

    var deleteResponse = await client.DeleteAsync("/api/1.0/CompanySettings");
    deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/CompanySettings");
    retrievalResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task DeleteNonExistentCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1000.ADMIN1");

    const long nonExistentId = 999999;
    var response = await client.DeleteAsync($"/api/1.0/CompanySettings/{nonExistentId}");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  public Task DisposeAsync() => Task.CompletedTask;

  [Fact]
  public async Task GetExistingCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.User1");

    var response = await client.GetAsync("/api/1.0/CompanySettings");
    response.StatusCode.ShouldBe(HttpStatusCode.OK);

    var responseModel = await response.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.CompanyId.ShouldBePositive();
  }

  [Fact]
  public async Task GetNonExistentCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1000.User1");

    var response = await client.GetAsync("/api/1.0/CompanySettings");
    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task UpdateExistingCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");

    const string newColorSchemeId = "updated-theme";

    var updateResponse = await client.PutAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(newColorSchemeId));

    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Verify the update
    var retrievalResponse = await client.GetAsync("/api/1.0/CompanySettings");
    retrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var responseModel = await retrievalResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.ColorSchemeId.ShouldBe(newColorSchemeId);
  }

  [Fact]
  public async Task UpdateNonExistentCompanySettingsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1000.ADMIN1");

    const long nonExistentId = 999999;
    var response = await client.PutAsJsonAsync($"/api/1.0/CompanySettings/{nonExistentId}",
      new CompanySettingsModificationModel("new-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }
}
