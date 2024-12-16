using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class BranchExtensions
{
  public static async Task SeedBranchesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await SeedBranchAsync(factory, "Branch1-1937190788", "America/New_York", "01JB0S0PAQTKGEWXBX1060B00T.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedBranchAsync(factory, "Branch2-1972002548", "America/New_York", "01JB0S0SYP3T4REGTTC3Y74N51.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedBranchAsync(factory, "Branch3-1513925028", "America/New_York", "01JB0S0SYP3T4REGTTC3Y74N51.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
  }

  private static async Task SeedBranchAsync<TEntryPoint>(
    WebApplicationFactory<TEntryPoint> factory,
    string branchName,
    string timeZoneId,
    string accessToken,
    CancellationToken cancellationToken) where TEntryPoint : class
  {
    var branchEasyStore = factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();

    if (!branchEasyStore.Entities.Values.Any(x => string.Equals(x.Name, branchName, StringComparison.Ordinal)))
    {
      var client = factory.CreateClient();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId), cancellationToken).ConfigureAwait(false);

      creationResponse.EnsureSuccessStatusCode();
    }
  }
}
