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

namespace Fossa.API.FunctionalTests.ControllerApis;

public class EmployeeControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public EmployeeControllerWithSystemLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task DeleteEmployeeWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync("/api/1.0/Employee");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingEmployeeWithoutDependenciesWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var employeeEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<EmployeeMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHFMA4ZXFE4EVMZARRTK7M.Tenant2.User2");

    employeeEasyStore.Entities.ContainsKey(20000L).ShouldBeTrue();

    var response = await client.DeleteAsync("/api/1.0/Employee");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    employeeEasyStore.Entities.ContainsKey(20000L).ShouldBeFalse();
  }

  [Fact]
  public async Task DeleteMissingEmployeeWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHG6056FSRFJ7GC9E21JFD.Tenant1000.User1");
    var response = await client.DeleteAsync("/api/1.0/Employee");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedCompaniesAsync(default).ConfigureAwait(false);
    await _factory.SeedBranchesAsync(default).ConfigureAwait(false);
    await _factory.SeedEmployeesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task RetrieveEmployeeWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/1.0/Employee");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingEmployeeWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHGBMC1DVP4R33QRYZ04RX.Tenant1.User1");
    var response = await client.GetAsync("/api/1.0/Employee");
    response.EnsureSuccessStatusCode();

    var responseModel =
      await response.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.CompanyId.ShouldBePositive();
    responseModel.FirstName.ShouldNotBeNullOrEmpty();
    responseModel.LastName.ShouldNotBeNullOrEmpty();
    responseModel.FullName.ShouldNotBeNullOrEmpty();
  }

  [Fact]
  public async Task RetrieveMissingEmployeeWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHGFXBG4WKNJKBSG7T2Y76.Tenant1000.User1000");
    var response = await client.GetAsync("/api/1.0/Employee");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }
}
