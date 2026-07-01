using System.Net;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.Bridge.Models.ApiModels;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanySettingsControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public CompanySettingsControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateCompanySettingsWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant3.ADMIN1");

    const string colorSchemeId = "new-theme";

    await settingsClient.createCompanySettingsAsync(new CompanySettingsModificationModel(colorSchemeId), TestContext.Current.CancellationToken);

    var responseModel = (await settingsClient.getCompanySettingsAsync(TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.id.ShouldBePositive();
    responseModel.companyId.ShouldBePositive();
    responseModel.colorSchemeId.ShouldBe(colorSchemeId);
  }

  [Fact]
  public async Task CreateCompanySettingsWithInvalidColorSchemeIdAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant4.ADMIN1");

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
      (await settingsClient.createCompanySettingsAsync(
        new CompanySettingsModificationModel(invalidColorSchemeId), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
    }
  }

  [Fact]
  public async Task CreateCompanySettingsWithValidColorSchemeIdFormatsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    // Test various valid colorSchemeId formats
    var validTestCases = new[]
    {
      new { Tenant = "Tenant45442385", colorSchemeId = "abc" }, // Minimum length
      new { Tenant = "Tenant45442386", colorSchemeId = "theme" }, // Simple lowercase
      new { Tenant = "Tenant45442387", colorSchemeId = "dark-theme" }, // With hyphen in middle
      new { Tenant = "Tenant45442388", colorSchemeId = "verylongthemename" }, // Long name
    };

    foreach (var testCase in validTestCases)
    {
      accessTokenContext.SetAccessToken($"01JA1ZJAWF27S0J8Z2VJE7673Y.{testCase.Tenant}.ADMIN1");

      const string companyName = "Company-1412593541";

      var transport = scope.ServiceProvider.GetRequiredService<IHttpTransport>();
      await transport.PostAsync<CompanyModificationModel>(
        "/api/1.0/Company",
        EndpointSecurity.RequireToken,
        new CompanyModificationModel(companyName, "US"),
        TestContext.Current.CancellationToken);

      await settingsClient.createCompanySettingsAsync(
        new CompanySettingsModificationModel(testCase.colorSchemeId), TestContext.Current.CancellationToken);

      var responseModel = (await settingsClient.getCompanySettingsAsync(TestContext.Current.CancellationToken)).Unwrap();

      responseModel.ShouldNotBeNull();
      responseModel.colorSchemeId.ShouldBe(testCase.colorSchemeId,
        $"ColorSchemeId '{testCase.colorSchemeId}' should be valid");
    }
  }

  [Fact]
  public async Task CreateDuplicateCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");

    // Try to create company settings for a company that already has settings
    (await settingsClient.createCompanySettingsAsync(
      new CompanySettingsModificationModel("duplicate-theme"), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Conflict);
  }

  [Fact]
  public async Task DeleteExistingCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JB0RAH24ZJBA53AJF5F5MMZX.Tenant2.ADMIN1");

    await settingsClient.deleteCompanySettingsAsync(TestContext.Current.CancellationToken);

    (await settingsClient.getCompanySettingsAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task DeleteNonExistentCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1000.ADMIN1");

    (await settingsClient.deleteCompanySettingsAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;

  [Fact]
  public async Task GetExistingCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.User1");

    var responseModel = (await settingsClient.getCompanySettingsAsync(TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.id.ShouldBePositive();
    responseModel.companyId.ShouldBePositive();
  }

  [Fact]
  public async Task GetNonExistentCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1000.User1");

    (await settingsClient.getCompanySettingsAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  [Fact]
  public async Task UpdateExistingCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");

    const string newColorSchemeId = "updated-theme";

    await settingsClient.updateCompanySettingsAsync(new CompanySettingsModificationModel(newColorSchemeId), TestContext.Current.CancellationToken);

    // Verify the update
    var responseModel = (await settingsClient.getCompanySettingsAsync(TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.colorSchemeId.ShouldBe(newColorSchemeId);
  }

  [Fact]
  public async Task UpdateNonExistentCompanySettingsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var settingsClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanySettingsClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1000.ADMIN1");

    (await settingsClient.updateCompanySettingsAsync(
      new CompanySettingsModificationModel("new-theme"), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }
}
