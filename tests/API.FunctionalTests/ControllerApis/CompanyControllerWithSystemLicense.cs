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

public class CompanyControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public CompanyControllerWithSystemLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task DeleteCompanyWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingCompanyWithDependenciesWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.ADMIN1");
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task DeleteExistingCompanyWithoutDependenciesWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var companyEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<CompanyMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant2.ADMIN1");

    companyEasyStore.Entities.ContainsKey(200L).ShouldBeTrue();

    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    companyEasyStore.Entities.ContainsKey(200L).ShouldBeFalse();
  }

  [Fact]
  public async Task DeleteExistingCompanyWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.User1");
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteMissingCompanyWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1000.ADMIN1");
    var response = await client.DeleteAsync("/api/1.0/Company");

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
  public async Task RetrieveCompanyWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingCompanyWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.User1");
    var response = await client.GetAsync("/api/1.0/Company");
    response.EnsureSuccessStatusCode();

    var responseModel =
      await response.Content.ReadFromJsonAsync<CompanyRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.Name.ShouldNotBeNullOrEmpty();
  }

  [Fact]
  public async Task RetrieveMissingCompanyWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SXMEMR1GQ3EE3Q4A872GKD.Tenant1000.User1000");
    var response = await client.GetAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }
}
