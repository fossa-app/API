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

public class BranchesControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public BranchesControllerWithSystemLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    responseModel.Items.Select(x => x.Name).ShouldContain(branchName);
  }

  [Fact]
  public async Task CreateBranchWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel("Branch X"));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteBranchWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync("/api/1.0/Branches/123456789");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingBranchWithoutDependenciesWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA0JKF0VRC9JPZ9JSAMHGAFS.Tenant1.ADMIN1");

    branchEasyStore.Entities.ContainsKey(2000L).ShouldBeTrue();

    var response = await client.DeleteAsync("/api/1.0/Branches/2000");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    branchEasyStore.Entities.ContainsKey(2000L).ShouldBeFalse();
  }

  [Fact]
  public async Task DeleteExistingBranchWithoutDependenciesWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA0JKF0VRC9JPZ9JSAMHGAFS.Tenant1.User2");

    branchEasyStore.Entities.ContainsKey(3000L).ShouldBeTrue();

    var response = await client.DeleteAsync("/api/1.0/Branches/3000");

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteMissingBranchWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1J99K3NCGNA6X4Z194PJXF.Tenant1.ADMIN1");
    var response = await client.DeleteAsync("/api/1.0/Branches/123456789");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  [Fact]
  public async Task DeleteMissingBranchWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA0JKPRJDN7RXSMGXZ946WRB.Tenant1000.User1");
    var response = await client.DeleteAsync("/api/1.0/Branches/123456789");

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task RetrieveBranchesWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=5");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingBranchesWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SYCJ4MHZXGQKT0ARG7KNCC.Tenant1.User1");
    var response = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=5");
    response.EnsureSuccessStatusCode();

    var responseModel =
      await response.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    responseModel.PageNumber.ShouldBe(1);
    responseModel.PageSize.ShouldBe(5);
    responseModel.Items.ShouldNotBeNull();
    responseModel.Items.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task RetrieveMissingBranchWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SYCPN31B53QHRR7Y13D30F.Tenant1000.User1000");
    var response = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=5");

    response.EnsureSuccessStatusCode();

    var responseModel =
      await response.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    responseModel.PageNumber.ShouldBe(1);
    responseModel.PageSize.ShouldBe(5);
    responseModel.Items.ShouldNotBeNull();
    responseModel.Items.ShouldBeEmpty();
  }
}
