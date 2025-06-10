using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanySettingsControllerValidationTests : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public CompanySettingsControllerValidationTests(
    CustomWebApplicationFactory<DefaultWebModule> factory,
    ITestOutputHelper testOutputHelper)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Theory]
  [InlineData("ab")] // Too short
  [InlineData("AB")] // Uppercase
  [InlineData("Test")] // Mixed case
  [InlineData("test123")] // Contains numbers
  [InlineData("test--theme")] // Double hyphen
  [InlineData("-test")] // Starts with hyphen
  [InlineData("test-")] // Ends with hyphen
  [InlineData("test theme")] // Contains space
  [InlineData("test_theme")] // Contains underscore
  [InlineData("test@theme")] // Contains special character
  [InlineData("test.theme")] // Contains dot
  [InlineData("")] // Empty string
  [InlineData("a")] // Single character
  [InlineData("12")] // Numbers only
  public async Task CreateCompanySettingsWithInvalidColorSchemeIdShouldFailAsync(string invalidColorSchemeId)
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant10.ADMIN1");

    var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(invalidColorSchemeId));

    if (response.StatusCode != HttpStatusCode.UnprocessableEntity)
    {
      var content = await response.Content.ReadAsStringAsync();
      _testOutputHelper.WriteLine($"Expected UnprocessableEntity for '{invalidColorSchemeId}', got {response.StatusCode}: {content}");
    }

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity,
      $"ColorSchemeId '{invalidColorSchemeId}' should be invalid");
  }

  [Theory]
  [InlineData("abc")] // Minimum valid length
  [InlineData("theme")] // Simple lowercase
  [InlineData("dark-theme")] // With hyphen in middle
  [InlineData("light-mode")] // Another hyphen example
  [InlineData("verylongthemename")] // Long name
  [InlineData("a-b")] // Minimum with hyphen
  [InlineData("simple")] // Simple word
  public async Task CreateCompanySettingsWithValidColorSchemeIdShouldSucceedAsync(string validColorSchemeId)
  {
    var client = _factory.CreateClient();
    var tenantSuffix = validColorSchemeId.GetHashCode().ToString().Replace("-", "");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant{tenantSuffix}.ADMIN1");

    var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(validColorSchemeId));

    if (response.StatusCode != HttpStatusCode.OK)
    {
      var content = await response.Content.ReadAsStringAsync();
      _testOutputHelper.WriteLine($"Expected OK for '{validColorSchemeId}', got {response.StatusCode}: {content}");
    }

    response.StatusCode.ShouldBe(HttpStatusCode.OK,
      $"ColorSchemeId '{validColorSchemeId}' should be valid");

    // Verify the created settings
    var retrievalResponse = await client.GetAsync("/api/1.0/CompanySettings");
    retrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var responseModel = await retrievalResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();
    responseModel.ShouldNotBeNull();
    responseModel.ColorSchemeId.ShouldBe(validColorSchemeId);
  }

  [Fact]
  public async Task CreateCompanySettingsWithNullColorSchemeIdShouldFailAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant11.ADMIN1");

    var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(null));

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateCompanySettingsWithInvalidColorSchemeIdShouldFailAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");

    // First create valid settings
    await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("valid-theme"));

    // Get the created settings ID
    var getResponse = await client.GetAsync("/api/1.0/CompanySettings");
    var settings = await getResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();

    // Try to update with invalid colorSchemeId
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/CompanySettings/{settings!.Id}",
      new CompanySettingsModificationModel("INVALID"));

    updateResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }
}
