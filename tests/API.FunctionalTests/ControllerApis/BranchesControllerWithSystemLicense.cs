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

public class BranchesControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public BranchesControllerWithSystemLicense(
    ITestOutputHelper testOutputHelper,
    CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithInvalidTimeZoneIdTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "USZone";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId));

    if (creationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await creationResponse.Content.ReadAsStringAsync());
    }

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithLicensedTimeZoneIdTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "America/New_York";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    responseModel.Items.Select(x => x.Name).ShouldContain(branchName);
    responseModel.Items.Select(x => x.TimeZoneId).ShouldContain(timeZoneId);
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithUnlicensedTimeZoneIdTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "Australia/Perth";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId));

    if (creationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await creationResponse.Content.ReadAsStringAsync());
    }

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateBranchWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel("Branch X", "America/New_York"));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithInvalidTimeZoneIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, "USZone"));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithLicensedTimeZoneIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, "America/Detroit"));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithUnlicensedTimeZoneIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, "Australia/Perth"));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateThenDeleteBranchWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-832159009";
    const string timeZoneId = "America/New_York";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    var branchRetrievalModel = responseModel.Items.Single(x => string.Equals(x.Name, branchName, StringComparison.OrdinalIgnoreCase));
    branchRetrievalModel.Name.ShouldBe(branchName);

    branchEasyStore.Entities.ContainsKey(branchRetrievalModel.Id).ShouldBeTrue();
    var deletionResponse = await client.DeleteAsync($"/api/1.0/Branches/{branchRetrievalModel.Id}");

    deletionResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    branchEasyStore.Entities.ContainsKey(branchRetrievalModel.Id).ShouldBeFalse();
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

    var existingBranch = branchEasyStore.Entities.Values.Single(x => string.Equals(x.Name, "Branch2-1972002548", StringComparison.Ordinal));

    var response = await client.DeleteAsync($"/api/1.0/Branches/{existingBranch.ID}");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    branchEasyStore.Entities.Values.FirstOrDefault(x => string.Equals(x.Name, "Branch2-1972002548", StringComparison.Ordinal)).ShouldBeNull();
  }

  [Fact]
  public async Task DeleteExistingBranchWithoutDependenciesWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA0JKF0VRC9JPZ9JSAMHGAFS.Tenant1.User2");

    var existingBranch = branchEasyStore.Entities.Values.Single(x => string.Equals(x.Name, "Branch3-1513925028", StringComparison.Ordinal));

    var response = await client.DeleteAsync($"/api/1.0/Branches/{existingBranch.ID}");

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

  [Fact]
  public async Task UpdateBranchWithAdministratorAccessWithLicensedTimeZoneIdTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string creationBranchName = "Branch-753988509";
    const string creationTimeZoneId = "America/New_York";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(creationBranchName, creationTimeZoneId));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    const string modificationBranchName = "Branch-509762905";
    const string modificationTimeZoneId = "America/Chicago";

    var modificationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(modificationBranchName, modificationTimeZoneId));

    modificationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    responseModel.Items.Select(x => x.Name).ShouldContain(modificationBranchName);
    responseModel.Items.Select(x => x.TimeZoneId).ShouldContain(modificationTimeZoneId);
  }
}
