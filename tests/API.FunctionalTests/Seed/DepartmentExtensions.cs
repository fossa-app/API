using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.Persistence.Mongo.Repositories;
using Fossa.API.Web.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class DepartmentExtensions
{
  public static async Task SeedDepartmentsAsync<TEntryPoint>(
      this WebApplicationFactory<TEntryPoint> factory,
      CancellationToken cancellationToken)
      where TEntryPoint : class
  {
    await SeedDepartmentAsync(factory, "Department1-274909726", 1, "01JSXF659A35D2EYN5V2YN0ZTK.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedDepartmentAsync(factory, "Department2-215942565", 1, "01JSXF659A35D2EYN5V2YN0ZTK.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
    await SeedDepartmentAsync(factory, "Department3-958996399", 1, "01JSXF659A35D2EYN5V2YN0ZTK.Tenant1.ADMIN1", cancellationToken).ConfigureAwait(false);
  }

  private static async Task SeedDepartmentAsync<TEntryPoint>(
      WebApplicationFactory<TEntryPoint> factory,
      string departmentName,
      long managerId,
      string accessToken,
      CancellationToken cancellationToken) where TEntryPoint : class
  {
    var departmentEasyStore = factory.Services.GetRequiredService<IEasyStores>().Resolve<DepartmentMongoEntity, long>();

    if (!departmentEasyStore.Entities.Values.Any(x => string.Equals(x.Name, departmentName, StringComparison.Ordinal)))
    {
      var client = factory.CreateClient();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

      var creationResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
          new DepartmentModificationModel(departmentName, null, managerId), cancellationToken).ConfigureAwait(false);

      creationResponse.EnsureSuccessStatusCode();
    }
  }
}
