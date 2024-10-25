using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class CompanyExtensions
{
  public static async Task SeedCompaniesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await SeedCompanyAsync(factory, "Company1-1587795889", "01JB0QS2K6SA4KYD8S920W7DMG.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedCompanyAsync(factory, "Company2-1031522025", "01JB0RAH24ZJBA53AJF5F5MMZX.Tenant2.ADMIN1", cancellationToken).ConfigureAwait(false);
  }

  private static async Task SeedCompanyAsync<TEntryPoint>(
    WebApplicationFactory<TEntryPoint> factory,
    string companyName,
    string accessToken,
    CancellationToken cancellationToken) where TEntryPoint : class
  {
    var companyEasyStore = factory.Services.GetRequiredService<IEasyStores>().Resolve<CompanyMongoEntity, long>();

    if (!companyEasyStore.Entities.Values.Any(x => string.Equals(x.Name, companyName, StringComparison.Ordinal)))
    {
      var client = factory.CreateClient();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      var creationResponse = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName), cancellationToken).ConfigureAwait(false);

      creationResponse.EnsureSuccessStatusCode();
    }
  }
}
