using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class CompanySettingsExtensions
{
  public static async Task SeedCompanySettingsAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await SeedCompanySettingsAsync(factory, 1, "theme-one", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedCompanySettingsAsync(factory, 2, "theme-two", "01JB0RAH24ZJBA53AJF5F5MMZX.Tenant2.ADMIN1", cancellationToken).ConfigureAwait(false);
  }

  private static async Task SeedCompanySettingsAsync<TEntryPoint>(
    WebApplicationFactory<TEntryPoint> factory,
    long companyId,
    string colorSchemeId,
    string accessToken,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    _ = accessToken; // Parameter required for consistency but not used in seeding
    var companySettingsEasyRepository = factory.Services.GetRequiredService<CompanySettingsMongoEasyRepository>();

    var companySettingsEntity = new CompanySettingsMongoEntity
    {
      ID = companyId,
      CompanyId = companyId,
      ColorSchemeId = colorSchemeId,
    };

    await companySettingsEasyRepository.TryAddAsync(companySettingsEntity, cancellationToken).ConfigureAwait(false);
  }
}
