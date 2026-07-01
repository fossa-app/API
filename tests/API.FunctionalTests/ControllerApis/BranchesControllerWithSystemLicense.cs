using System.Net;
using System.Net.Http.Headers;
using EasyDoubles;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web;
using Fossa.Bridge.Models.ApiModels;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class BranchesControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  public BranchesControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithInvalidTimeZoneIdTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "USZone";
    var address = new AddressModel("1234 Main St", "Suite 100", "Los Angeles", "CA", "12345", "US");

    (await branchClient.createBranchAsync(new BranchModificationModel(branchName, timeZoneId, address), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithLicensedTimeZoneIdTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "America/New_York";

    await branchClient.createBranchAsync(new BranchModificationModel(branchName, timeZoneId, address: null), TestContext.Current.CancellationToken);

    var responseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.items.Select(x => x.name).ShouldContain(branchName);
    responseModel.items.Single(x => string.Equals(x.name, branchName, StringComparison.OrdinalIgnoreCase)).timeZoneId.ShouldBe(timeZoneId);
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithUnlicensedTimeZoneIdTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "Australia/Perth";

    (await branchClient.createBranchAsync(new BranchModificationModel(branchName, timeZoneId, address: null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateBranchWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;

    (await branchClient.createBranchAsync(new BranchModificationModel("Branch X", "America/New_York", address: null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithInvalidTimeZoneIdAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";

    (await branchClient.createBranchAsync(new BranchModificationModel(branchName, "USZone", address: null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithLicensedTimeZoneIdAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";

    (await branchClient.createBranchAsync(new BranchModificationModel(branchName, "America/Detroit", address: null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithUnlicensedTimeZoneIdAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";

    (await branchClient.createBranchAsync(new BranchModificationModel(branchName, "Australia/Perth", address: null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateThenDeleteBranchWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-832159009";
    const string timeZoneId = "America/New_York";

    await branchClient.createBranchAsync(new BranchModificationModel(branchName, timeZoneId, address: null), TestContext.Current.CancellationToken);

    var responseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    var branchRetrievalModel = responseModel.items.Single(x => string.Equals(x.name, branchName, StringComparison.OrdinalIgnoreCase));
    branchRetrievalModel.name.ShouldBe(branchName);

    branchEasyStore.Entities.ContainsKey(branchRetrievalModel.id).ShouldBeTrue();
    await branchClient.deleteBranchAsync(branchRetrievalModel.id, TestContext.Current.CancellationToken);

    branchEasyStore.Entities.ContainsKey(branchRetrievalModel.id).ShouldBeFalse();
  }

  [Fact]
  public async Task DeleteBranchWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;

    (await branchClient.deleteBranchAsync(123456789, TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingBranchWithDependenciesWithAdministratorAccessTokenAsync()
  {
    // Arrange

    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    const string branchName = "Branch-35291729";
    const string timeZoneId = "America/New_York";

    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JKXJVFFPWRP9E7YNBQE8KMRB.Tenant1.User35292075");

    await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var employeeResponseModel = (await employeeClient.getEmployeesAsync(new EmployeeQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    employeeResponseModel.ShouldNotBeNull();
    var createdEmployee = employeeResponseModel.items.First();

    accessTokenContext.SetAccessToken("01JKXHHECNDQ6BYNA6CQQ2S59P.Tenant1.ADMIN1");

    await branchClient.createBranchAsync(new BranchModificationModel(branchName, timeZoneId, address: null), TestContext.Current.CancellationToken);

    var branchResponseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    branchResponseModel.ShouldNotBeNull();

    var createdBranchModel = branchResponseModel.items.Single(x => string.Equals(x.name, branchName, StringComparison.OrdinalIgnoreCase));

    var transport = scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    await transport.PutAsync<EmployeeManagementModel>(
      $"/api/1.0/Employees/{createdEmployee.id}",
      EndpointSecurity.RequireToken,
      new EmployeeManagementModel(createdBranchModel.id, null, null, "Staff"),
      TestContext.Current.CancellationToken);

    // Act

    (await branchClient.deleteBranchAsync(createdBranchModel.id, TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task DeleteExistingBranchWithoutDependenciesWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();

    accessTokenContext.SetAccessToken("01JA0JKF0VRC9JPZ9JSAMHGAFS.Tenant1.ADMIN1");

    var existingBranch = branchEasyStore.Entities.Values.Single(x => string.Equals(x.Name, "Branch2-1972002548", StringComparison.Ordinal));

    await branchClient.deleteBranchAsync(existingBranch.ID, TestContext.Current.CancellationToken);

    branchEasyStore.Entities.Values.FirstOrDefault(x => string.Equals(x.Name, "Branch2-1972002548", StringComparison.Ordinal)).ShouldBeNull();
  }

  [Fact]
  public async Task DeleteExistingBranchWithoutDependenciesWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    var branchEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<BranchMongoEntity, long>();

    accessTokenContext.SetAccessToken("01JA0JKF0VRC9JPZ9JSAMHGAFS.Tenant1.User2");

    var existingBranch = branchEasyStore.Entities.Values.Single(x => string.Equals(x.Name, "Branch3-1513925028", StringComparison.Ordinal));

    (await branchClient.deleteBranchAsync(existingBranch.ID, TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteMissingBranchWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1J99K3NCGNA6X4Z194PJXF.Tenant1.ADMIN1");

    await Should.NotThrowAsync(async () =>
      await branchClient.deleteBranchAsync(123456789, TestContext.Current.CancellationToken));
  }

  [Fact]
  public async Task DeleteMissingBranchWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA0JKPRJDN7RXSMGXZ946WRB.Tenant1000.User1");

    (await branchClient.deleteBranchAsync(123456789, TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Forbidden);
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  [Fact]
  public async Task ListBranches_WithSearchTerm_ReturnsFilteredResultsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    // Create branches with distinct names
    await branchClient.createBranchAsync(new BranchModificationModel("NYC Downtown Branch", "America/New_York", null), TestContext.Current.CancellationToken);
    await branchClient.createBranchAsync(new BranchModificationModel("LA Downtown Branch", "America/Los_Angeles", null), TestContext.Current.CancellationToken);
    await branchClient.createBranchAsync(new BranchModificationModel("NYC Uptown Branch", "America/New_York", null), TestContext.Current.CancellationToken);

    // Search for NYC branches
    var result = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { search = "NYC", pageNumber = 1, pageSize = 10 }, TestContext.Current.CancellationToken)).Unwrap();

    result.ShouldNotBeNull();
    result.items.Count.ShouldBe(2);
    result.items.All(x => x.name?.Contains("NYC") == true).ShouldBeTrue();
  }

  [Fact]
  public async Task ListBranches_WithSpecificIds_ReturnsRequestedBranchesAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    // Create branches and collect their IDs
    var branchIds = new List<long>();
    for (int i = 1; i <= 3; i++)
    {
      await branchClient.createBranchAsync(new BranchModificationModel($"Test Branch {i}", "America/New_York", null), TestContext.Current.CancellationToken);

      var branches = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
      branches.ShouldNotBeNull();
      var branch = branches.items.First(x => x.name == $"Test Branch {i}");
      branchIds.Add(branch.id);
    }

    // Request specific branches by ID
    var result = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { id = [branchIds[0], branchIds[1]] }, TestContext.Current.CancellationToken)).Unwrap();

    result.ShouldNotBeNull();
    result.items.Count.ShouldBe(2);
    result.items.Select(x => x.id).ShouldContain(branchIds[0]);
    result.items.Select(x => x.id).ShouldContain(branchIds[1]);
    result.items.Select(x => x.id).ShouldNotContain(branchIds[2]);
  }

  [Theory]
  [InlineData(1, 5)]
  [InlineData(2, 3)]
  [InlineData(1, 10)]
  public async Task ListBranches_WithValidPaging_ReturnsPaginatedResultsAsync(int pageNumber, int pageSize)
  {
    // Create multiple branches
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    for (int i = 1; i <= 12; i++)
    {
      await branchClient.createBranchAsync(new BranchModificationModel($"Test Branch {i}", "America/New_York", null), TestContext.Current.CancellationToken);
    }

    // Get paginated results
    var result = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = pageNumber, pageSize = pageSize }, TestContext.Current.CancellationToken)).Unwrap();

    result.ShouldNotBeNull();
    result.pageNumber.ShouldBe(pageNumber);
    result.pageSize.ShouldBe(pageSize);
    result.items.Count.ShouldBeLessThanOrEqualTo(pageSize);
  }

  [Fact]
  public async Task ListCreatedBranchesAsync()
  {
    // Arrange

    const string branch1Name = "Branch-1832333622";
    const string branch2Name = "Branch-806632548";
    const string branch3Name = "Branch-637183497";
    const string timeZoneId = "America/New_York";

    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JMV0X5W7N908QW69WVVDPFAW.Tenant1.ADMIN1");

    await branchClient.createBranchAsync(new BranchModificationModel(branch1Name, timeZoneId, address: null), TestContext.Current.CancellationToken);
    await branchClient.createBranchAsync(new BranchModificationModel(branch2Name, timeZoneId, address: null), TestContext.Current.CancellationToken);
    await branchClient.createBranchAsync(new BranchModificationModel(branch3Name, timeZoneId, address: null), TestContext.Current.CancellationToken);

    accessTokenContext.SetAccessToken("01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.User1");

    var retrievalResponseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    retrievalResponseModel.ShouldNotBeNull();

    var branch1Id = retrievalResponseModel.items.Single(x => x.name == branch1Name).id;
    var branch2Id = retrievalResponseModel.items.Single(x => x.name == branch2Name).id;
    var branch3Id = retrievalResponseModel.items.Single(x => x.name == branch3Name).id;
    const int branch4Id = 204298046; // Missing branch

    // Act

    var branchResponseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { id = [branch1Id, branch2Id, branch3Id, branch4Id] }, TestContext.Current.CancellationToken)).Unwrap();

    // Assert

    branchResponseModel.ShouldNotBeNull();

    branchResponseModel.items.Count.ShouldBe(3);
    branchResponseModel.items.ShouldContain(x => x.id == branch1Id);
    branchResponseModel.items.ShouldContain(x => x.id == branch2Id);
    branchResponseModel.items.ShouldContain(x => x.id == branch3Id);
    branchResponseModel.items.ShouldNotContain(x => x.id == branch4Id);
  }

  [Fact]
  public async Task RetrieveBranchesWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;

    (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 5 }, TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingBranchesWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9SYCJ4MHZXGQKT0ARG7KNCC.Tenant1.User1");
    var responseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 5 }, TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.pageNumber.ShouldBe(1);
    responseModel.pageSize.ShouldBe(5);
    responseModel.items.ShouldNotBeNull();
    responseModel.items.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task RetrieveMissingBranchWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9SYCPN31B53QHRR7Y13D30F.Tenant1000.User1000");
    var responseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 5 }, TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.pageNumber.ShouldBe(1);
    responseModel.pageSize.ShouldBe(5);
    responseModel.items.ShouldNotBeNull();
    responseModel.items.ShouldBeEmpty();
  }

  [Theory]
  [InlineData("5234 Main St", "Suite 200", "New York", "NY", "62345", "USA")]
  [InlineData("5234 Main St", "Suite 200", "New York", "NY", "62345", "CA")]
  [InlineData("5234 Main St", "Suite 200", "New York", "NY", "", "US")]
  [InlineData("5234 Main St", "Suite 200", "New York", "", "62345", "US")]
  [InlineData("5234 Main St", "Suite 200", "", "NY", "62345", "US")]
  [InlineData("", "Suite 200", "New York", "NY", "62345", "US")]
  [InlineData("5234 Main St", "Suite 200", "New York", "NY", "62", "US")]
  [InlineData("5234 Main St", "Suite 200", "New York", "NY", "62345-", "US")]
  [InlineData("5234 Main St", "Suite 200", "New York", "NY", "62345-12", "US")]
  public async Task UpdateBranchWithAdministratorAccessTokenWithInvalidAddressAsync(
    string? line1, string? line2, string? city, string? subdivision, string? postalCode, string? countryCode)
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    var creationBranchName = $"Branch-{Random.Shared.Next()}";
    const string creationTimeZoneId = "America/Los_Angeles";
    var creationAddress = new AddressModel("1234 Main St", "Suite 100", "Los Angeles", "CA", "12345", "US");

    await branchClient.createBranchAsync(new BranchModificationModel(creationBranchName, creationTimeZoneId, creationAddress), TestContext.Current.CancellationToken);

    var response1Model = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    response1Model.ShouldNotBeNull();
    var creationBranch = response1Model.items.Single(x => string.Equals(x.name, creationBranchName, StringComparison.OrdinalIgnoreCase));

    var modificationBranchName = $"Branch-{Random.Shared.Next()}";
    const string modificationTimeZoneId = "America/New_York";
    var modificationAddress = new AddressModel(line1, line2, city, subdivision, postalCode, countryCode);

    (await branchClient.updateBranchAsync(creationBranch.id, new BranchModificationModel(modificationBranchName, modificationTimeZoneId, modificationAddress), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateBranchWithAdministratorAccessWithLicensedTimeZoneIdTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string creationBranchName = "Branch-753988509";
    const string creationTimeZoneId = "America/Los_Angeles";
    var creationAddress = new AddressModel("1234 Main St", "Suite 100", "Los Angeles", "CA", "12345", "US");

    await branchClient.createBranchAsync(new BranchModificationModel(creationBranchName, creationTimeZoneId, creationAddress), TestContext.Current.CancellationToken);

    var response1Model = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    response1Model.ShouldNotBeNull();
    var creationBranch = response1Model.items.Single(x => string.Equals(x.name, creationBranchName, StringComparison.OrdinalIgnoreCase));

    const string modificationBranchName = "Branch-509762905";
    const string modificationTimeZoneId = "America/New_York";
    var modificationAddress = new AddressModel("5234 Main St", "Suite 200", "New York", "NY", "62345", "US");

    await branchClient.updateBranchAsync(creationBranch.id, new BranchModificationModel(modificationBranchName, modificationTimeZoneId, modificationAddress), TestContext.Current.CancellationToken);

    var response2Model = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    response2Model.ShouldNotBeNull();
    response2Model.items.Select(x => x.name).ShouldContain(modificationBranchName);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).timeZoneId.ShouldBe(modificationTimeZoneId);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address.ShouldNotBeNull();
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address?.line1.ShouldBe(modificationAddress.line1);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address?.line2.ShouldBe(modificationAddress.line2);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address?.city.ShouldBe(modificationAddress.city);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address?.subdivision.ShouldBe(modificationAddress.subdivision);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address?.postalCode.ShouldBe(modificationAddress.postalCode);
    response2Model.items.Single(x => string.Equals(x.name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).address?.countryCode.ShouldBe(modificationAddress.countryCode);
  }

  [Theory]
  [InlineData(0, 10)]   // Invalid page number
  [InlineData(1, 0)]    // Invalid page size
  [InlineData(-1, 10)]  // Negative page number
  [InlineData(1, -1)]   // Negative page size
  [InlineData(1, 1001)] // Page size too large
  public async Task ListBranches_WithInvalidPaging_ReturnsUnprocessableEntityAsync(int pageNumber, int pageSize)
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    var response = await client.GetAsync($"/api/1.0/Branches?pageNumber={pageNumber}&pageSize={pageSize}", TestContext.Current.CancellationToken);

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateBranch_WithValidData_SucceedsAsync()
  {
    // Create a branch first
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    const string branchName = "Test Branch 647834591";
    await branchClient.createBranchAsync(new BranchModificationModel(branchName, "America/New_York", null), TestContext.Current.CancellationToken);

    // Get the branch ID
    var branches = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    branches.ShouldNotBeNull();
    var branch = branches.items.First(x => x.name == branchName);

    // Update the branch
    await branchClient.updateBranchAsync(branch.id, new BranchModificationModel("Updated Branch", "America/Chicago", new AddressModel("123 Main St", null, "Chicago", "IL", "60601", "US")), TestContext.Current.CancellationToken);

    // Verify the update
    var updatedBranch = (await branchClient.getBranchAsync(branch.id, TestContext.Current.CancellationToken)).Unwrap();

    updatedBranch.ShouldNotBeNull();
    updatedBranch.name.ShouldBe("Updated Branch");
    updatedBranch.timeZoneId.ShouldBe("America/Chicago");
    updatedBranch.address.ShouldNotBeNull();
    updatedBranch.address.city.ShouldBe("Chicago");
  }

  [Fact]
  public async Task UpdateBranch_WithInvalidTimeZone_ReturnsUnprocessableEntityAsync()
  {
    // Create a branch first
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    const string branchName = "Test Branch 984679490";
    await branchClient.createBranchAsync(new BranchModificationModel(branchName, "America/New_York", null), TestContext.Current.CancellationToken);

    var branches = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    branches.ShouldNotBeNull();
    var branch = branches.items.First(x => x.name == branchName);

    // Try to update with invalid timezone
    (await branchClient.updateBranchAsync(branch.id, new BranchModificationModel("Updated Branch", "Invalid/TimeZone", null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateBranch_WithInvalidBranchId_ReturnsNotFoundAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    (await branchClient.updateBranchAsync(999999, new BranchModificationModel("Updated Branch", "America/Chicago", null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UpdateBranch_WithoutAuthorization_ReturnsUnauthorizedAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;

    (await branchClient.updateBranchAsync(1, new BranchModificationModel("Updated Branch", "America/Chicago", null), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task UpdateBranchWithDifferentCountryCodeShouldFailAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var branchClient = scope.ServiceProvider.GetRequiredService<IClients>().BranchClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");

    // Create initial branch with US address
    const string branchName = "Branch-CountryValidation";
    const string timeZoneId = "America/New_York";
    var initialAddress = new AddressModel("1234 Main St", "Suite 100", "New York", "NY", "10001", "US");

    await branchClient.createBranchAsync(new BranchModificationModel(branchName, timeZoneId, initialAddress), TestContext.Current.CancellationToken);

    // Get the created branch ID
    var responseModel = (await branchClient.getBranchesAsync(new BranchQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    responseModel.ShouldNotBeNull();
    var createdBranch = responseModel.items.Single(x => string.Equals(x.name, branchName, StringComparison.OrdinalIgnoreCase));

    // Attempt to update with Canadian address
    var canadianAddress = new AddressModel("456 Maple St", null, "Toronto", "ON", "M5V 2H1", "CA");
    (await branchClient.updateBranchAsync(createdBranch.id, new BranchModificationModel(branchName, timeZoneId, canadianAddress), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);

    // Verify the branch still has US address
    var verificationModel = (await branchClient.getBranchAsync(createdBranch.id, TestContext.Current.CancellationToken)).Unwrap();
    verificationModel.ShouldNotBeNull();
    verificationModel.address.ShouldNotBeNull();
    verificationModel.address?.countryCode.ShouldBe("US");
  }
}
