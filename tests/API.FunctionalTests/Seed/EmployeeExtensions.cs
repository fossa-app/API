using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class EmployeeExtensions
{
  public static async Task SeedEmployeesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await SeedEmployeeAsync(factory, "Bill", "Gates", "01JHY9QQQB55BM6D3Y9PZQ22TD.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedEmployeeAsync(factory, "Steve", "Ballmer", "01JHY9QWEWVJFPN25TKCA942NX.Tenant1.ADMIN2", cancellationToken).ConfigureAwait(false);
    await SeedEmployeeAsync(factory, "Paul", "Allen", "01JB0S0SYP3T4REGTTC3Y74N51.Tenant1.User1", cancellationToken).ConfigureAwait(false);
    await SeedEmployeeAsync(factory, "Jim", "Allchin", "01JB0S0SYP3T4REGTTC3Y74N51.Tenant1.User2", cancellationToken).ConfigureAwait(false);
  }

  private static async Task SeedEmployeeAsync<TEntryPoint>(
    WebApplicationFactory<TEntryPoint> factory,
    string employeeFirstName,
    string employeeLastName,
    string accessToken,
    CancellationToken cancellationToken) where TEntryPoint : class
  {
    var employeeFullName = $"{employeeFirstName} {employeeLastName}";
    var employeeEasyStore = factory.Services.GetRequiredService<IEasyStores>().Resolve<EmployeeMongoEntity, long>();

    if (!employeeEasyStore.Entities.Values.Any(x => string.Equals(x.FullName, employeeFullName, StringComparison.Ordinal)))
    {
      var client = factory.CreateClient();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      var creationResponse = await client.PostAsJsonAsync(
        "/api/1.0/Employee",
        new EmployeeModificationModel(
          employeeFirstName,
          employeeLastName,
          employeeFullName),
        cancellationToken).ConfigureAwait(false);

      creationResponse.EnsureSuccessStatusCode();
    }
  }
}
