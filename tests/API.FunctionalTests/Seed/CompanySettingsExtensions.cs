using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Fossa.API.FunctionalTests.Seed;

public static class CompanySettingsExtensions
{
  public static async Task SeedCompanySettingsAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await SeedCompanySettingsAsync(factory, "theme-one", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedCompanySettingsAsync(factory, "theme-two", "01JB0RAH24ZJBA53AJF5F5MMZX.Tenant2.ADMIN1", cancellationToken).ConfigureAwait(false);
  }

  private static async Task SeedCompanySettingsAsync<TEntryPoint>(
    WebApplicationFactory<TEntryPoint> factory,
    string colorSchemeId,
    string accessToken,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    var client = factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var companySettingsRetrievalResponse = await client.GetAsync("/api/1.0/CompanySettings", cancellationToken);

    if (companySettingsRetrievalResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      var existingCompanySettings = await companySettingsRetrievalResponse.Content.ReadFromJsonAsync<CompanySettingsMongoEntity>(cancellationToken);
      existingCompanySettings.ShouldNotBeNull();

      var creationResponse = await client.PostAsJsonAsync("/api/1.0/CompanySettings", new CompanySettingsModificationModel(colorSchemeId), cancellationToken);
      creationResponse.ShouldNotBeNull();
    }
  }
}
