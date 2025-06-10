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

public class CompanySettingsControllerEdgeCaseTests : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public CompanySettingsControllerEdgeCaseTests(
    CustomWebApplicationFactory<DefaultWebModule> factory,
    ITestOutputHelper testOutputHelper)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    _ = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Fact]
  public async Task CreateCompanySettingsForCompanyWithoutLicenseShouldFailAsync()
  {
    var client = _factory.CreateClient();
    // Use a tenant that doesn't have a company license
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.TenantWithoutLicense.ADMIN1");

    var response = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("test-theme"));

    // Should fail because there's no company for this tenant
    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

    var content = await response.Content.ReadAsStringAsync();
    content.ShouldContain("A company must exist for this tenant before creating company settings");
  }

  [Fact]
  public async Task CreateCompanySettingsWhenAlreadyExistShouldFailAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");

    // First creation should succeed
    var firstResponse = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("first-theme"));
    firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Second creation should fail with validation error
    var secondResponse = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("second-theme"));
    secondResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

    var content = await secondResponse.Content.ReadAsStringAsync();
    content.ShouldContain("Company settings for this company have already been created");
  }

  [Fact]
  public async Task ConcurrentCreateCompanySettingsShouldHandleGracefullyAsync()
  {
    var client1 = _factory.CreateClient();
    var client2 = _factory.CreateClient();

    // Both clients use the same tenant
    const string accessToken = "01JA1ZJAWF27S0J8Z2VJE7673Y.TenantConcurrent.ADMIN1";
    client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    // Create company settings concurrently
    var task1 = client1.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("theme-one"));
    var task2 = client2.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("theme-two"));

    var responses = await Task.WhenAll(task1, task2);

    // One should succeed, one should fail with conflict
    var successCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
    var conflictCount = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

    successCount.ShouldBe(1);
    conflictCount.ShouldBe(1);
  }

  [Fact]
  public async Task UpdateNonExistentCompanySettingsWithValidIdShouldReturn404Async()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant12.ADMIN1");

    const long nonExistentId = 999999999;
    var response = await client.PutAsJsonAsync($"/api/1.0/CompanySettings/{nonExistentId}",
      new CompanySettingsModificationModel("new-theme"));

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task DeleteNonExistentCompanySettingsWithValidIdShouldReturn404Async()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant13.ADMIN1");

    const long nonExistentId = 999999999;
    var response = await client.DeleteAsync($"/api/1.0/CompanySettings/{nonExistentId}");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task CreateUpdateDeleteLifecycleTestAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.TenantLifecycle.ADMIN1");

    // 1. Create company settings
    const string initialTheme = "initial-theme";
    var createResponse = await client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(initialTheme));
    createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // 2. Verify creation
    var getResponse = await client.GetAsync("/api/1.0/CompanySettings");
    getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var settings = await getResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();
    settings.ShouldNotBeNull();
    settings.ColorSchemeId.ShouldBe(initialTheme);

    // 3. Update company settings
    const string updatedTheme = "updated-theme";
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/CompanySettings/{settings.Id}",
      new CompanySettingsModificationModel(updatedTheme));
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // 4. Verify update
    var getUpdatedResponse = await client.GetAsync("/api/1.0/CompanySettings");
    getUpdatedResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var updatedSettings = await getUpdatedResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();
    updatedSettings.ShouldNotBeNull();
    updatedSettings.ColorSchemeId.ShouldBe(updatedTheme);

    // 5. Delete company settings
    var deleteResponse = await client.DeleteAsync($"/api/1.0/CompanySettings/{settings.Id}");
    deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // 6. Verify deletion
    var getDeletedResponse = await client.GetAsync("/api/1.0/CompanySettings");
    getDeletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task MultipleUsersFromSameTenantShouldAccessSameCompanySettingsAsync()
  {
    var adminClient = _factory.CreateClient();
    var userClient = _factory.CreateClient();

    adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");
    userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.User1");

    // Admin creates settings
    const string themeId = "shared-theme";
    var createResponse = await adminClient.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel(themeId));
    createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // User should be able to read the same settings
    var userGetResponse = await userClient.GetAsync("/api/1.0/CompanySettings");
    userGetResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var userSettings = await userGetResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();
    userSettings.ShouldNotBeNull();
    userSettings.ColorSchemeId.ShouldBe(themeId);

    // Admin should see the same settings
    var adminGetResponse = await adminClient.GetAsync("/api/1.0/CompanySettings");
    adminGetResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var adminSettings = await adminGetResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();
    adminSettings.ShouldNotBeNull();
    adminSettings.Id.ShouldBe(userSettings.Id);
    adminSettings.CompanyId.ShouldBe(userSettings.CompanyId);
    adminSettings.ColorSchemeId.ShouldBe(userSettings.ColorSchemeId);
  }

  [Fact]
  public async Task UpdateCompanySettingsFromDifferentTenantShouldFailAsync()
  {
    var tenant1Client = _factory.CreateClient();
    var tenant2Client = _factory.CreateClient();

    tenant1Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");
    tenant2Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant2.ADMIN1");

    // Tenant 1 creates settings
    var createResponse = await tenant1Client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("tenant1-theme"));
    createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Get the created settings ID
    var getResponse = await tenant1Client.GetAsync("/api/1.0/CompanySettings");
    var settings = await getResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();

    // Tenant 2 tries to update Tenant 1's settings - should fail
    var updateResponse = await tenant2Client.PutAsJsonAsync($"/api/1.0/CompanySettings/{settings!.Id}",
      new CompanySettingsModificationModel("malicious-theme"));

    updateResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    var content = await updateResponse.Content.ReadAsStringAsync();
    content.ShouldContain("Company settings must exist and belong to a company in the same tenant");
  }

  [Fact]
  public async Task DeleteCompanySettingsFromDifferentTenantShouldFailAsync()
  {
    var tenant1Client = _factory.CreateClient();
    var tenant2Client = _factory.CreateClient();

    tenant1Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1");
    tenant2Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant2.ADMIN1");

    // Tenant 1 creates settings
    var createResponse = await tenant1Client.PostAsJsonAsync("/api/1.0/CompanySettings",
      new CompanySettingsModificationModel("tenant1-theme"));
    createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Get the created settings ID
    var getResponse = await tenant1Client.GetAsync("/api/1.0/CompanySettings");
    var settings = await getResponse.Content.ReadFromJsonAsync<CompanySettingsRetrievalModel>();

    // Tenant 2 tries to delete Tenant 1's settings - should fail
    var deleteResponse = await tenant2Client.DeleteAsync($"/api/1.0/CompanySettings/{settings!.Id}");

    deleteResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    var content = await deleteResponse.Content.ReadAsStringAsync();
    content.ShouldContain("Company settings must exist and belong to a company in the same tenant");
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }
}
