﻿using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.FunctionalTests.Extensions;
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
    var address = new AddressModel("1234 Main St", "Suite 100", "Los Angeles", "CA", "12345", "US");

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId, address));

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

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId, Address: null));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    responseModel.ShouldNotBeNull();
    responseModel.Items.Select(x => x.Name).ShouldContain(branchName);
    responseModel.Items.Single(x => string.Equals(x.Name, branchName, StringComparison.OrdinalIgnoreCase)).TimeZoneId.ShouldBe(timeZoneId);
  }

  [Fact]
  public async Task CreateBranchWithAdministratorAccessWithUnlicensedTimeZoneIdTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string branchName = "Branch-392136901";
    const string timeZoneId = "Australia/Perth";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId, Address: null));

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

    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel("Branch X", "America/New_York", Address: null));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithInvalidTimeZoneIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, "USZone", Address: null));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithLicensedTimeZoneIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, "America/Detroit", Address: null));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task CreateBranchWithUserAccessTokenWithUnlicensedTimeZoneIdAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.User1");
    const string branchName = "Branch-826076795";
    var response = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, "Australia/Perth", Address: null));

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

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId, Address: null));

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
  public async Task DeleteExistingBranchWithDependenciesWithAdministratorAccessTokenAsync()
  {
    // Arrange

    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    const string branchName = "Branch-35291729";
    const string timeZoneId = "America/New_York";

    var client = _factory.CreateClient();

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JKXJVFFPWRP9E7YNBQE8KMRB.Tenant1.User35292075");

    var employeeCreationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));

    employeeCreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var employeeRetrievalResponse = await client.GetAsync("/api/1.0/Employee");

    var employeeResponseModel =
      await employeeRetrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();

    employeeResponseModel.ShouldNotBeNull();

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JKXHHECNDQ6BYNA6CQQ2S59P.Tenant1.ADMIN1");

    var branchCreationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branchName, timeZoneId, Address: null));

    branchCreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var branchRetrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var branchResponseModel =
      await branchRetrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    branchResponseModel.ShouldNotBeNull();

    var createdBranchModel = branchResponseModel.Items.Single(x => string.Equals(x.Name, branchName, StringComparison.OrdinalIgnoreCase));

    var employeeManagementResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employeeResponseModel?.Id}", new EmployeeManagementModel(createdBranchModel.Id));

    employeeManagementResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Act

    var response = await client.DeleteAsync($"/api/1.0/Branches/{createdBranchModel.Id}");

    // Assert

    await _testOutputHelper.WriteAsync(response);

    response.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
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
  public async Task ListCreatedBranchesAsync()
  {
    // Arrange

    const string branch1Name = "Branch-1832333622";
    const string branch2Name = "Branch-806632548";
    const string branch3Name = "Branch-637183497";
    const string timeZoneId = "America/New_York";

    var client = _factory.CreateClient();

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JMV0X5W7N908QW69WVVDPFAW.Tenant1.ADMIN1");

    var branch1CreationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branch1Name, timeZoneId, Address: null));
    var branch2CreationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branch2Name, timeZoneId, Address: null));
    var branch3CreationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(branch3Name, timeZoneId, Address: null));

    branch1CreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    branch2CreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    branch3CreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.User1");

    var retrievalResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    if (retrievalResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await retrievalResponse.Content.ReadAsStringAsync());
    }
    retrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    retrievalResponseModel.ShouldNotBeNull();

    var branch1CreationResponseModel = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, branch1Name, StringComparison.OrdinalIgnoreCase));
    var branch2CreationResponseModel = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, branch2Name, StringComparison.OrdinalIgnoreCase));
    var branch3CreationResponseModel = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, branch3Name, StringComparison.OrdinalIgnoreCase));

    var branch1Id = branch1CreationResponseModel?.Id;
    var branch2Id = branch2CreationResponseModel?.Id;
    var branch3Id = branch3CreationResponseModel?.Id;
    const int branch4Id = 204298046; // Missing branch

    // Act

    var branchRetrievalResponse = await client.GetAsync($"/api/1.0/Branches?Id={branch1Id}&Id={branch2Id}&Id={branch3Id}&Id={branch4Id}");

    if (branchRetrievalResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await branchRetrievalResponse.Content.ReadAsStringAsync());
    }
    branchRetrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var branchResponseModel =
      await branchRetrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    // Assert

    branchResponseModel.ShouldNotBeNull();

    branchResponseModel.Items.Count.ShouldBe(3);
    branchResponseModel.Items.ShouldContain(x => x.Id == branch1Id);
    branchResponseModel.Items.ShouldContain(x => x.Id == branch2Id);
    branchResponseModel.Items.ShouldContain(x => x.Id == branch3Id);
    branchResponseModel.Items.ShouldNotContain(x => x.Id == branch4Id);
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
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    var creationBranchName = $"Branch-{Random.Shared.Next()}";
    const string creationTimeZoneId = "America/Los_Angeles";
    var creationAddress = new AddressModel("1234 Main St", "Suite 100", "Los Angeles", "CA", "12345", "US");

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(creationBranchName, creationTimeZoneId, creationAddress));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse1 = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var response1Model =
      await retrievalResponse1.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();
    response1Model.ShouldNotBeNull();
    var creationBranch = response1Model.Items.Single(x => string.Equals(x.Name, creationBranchName, StringComparison.OrdinalIgnoreCase));

    var modificationBranchName = $"Branch-{Random.Shared.Next()}";
    const string modificationTimeZoneId = "America/New_York";
    var modificationAddress = new AddressModel(line1, line2, city, subdivision, postalCode, countryCode);

    var modificationResponse = await client.PutAsJsonAsync($"/api/1.0/Branches/{creationBranch?.Id}", new BranchModificationModel(modificationBranchName, modificationTimeZoneId, modificationAddress));

    await _testOutputHelper.WriteAsync(modificationResponse);

    modificationResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateBranchWithAdministratorAccessWithLicensedTimeZoneIdTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9WMVQRX3J3K00JCDGZN4V59.Tenant1.ADMIN1");
    const string creationBranchName = "Branch-753988509";
    const string creationTimeZoneId = "America/Los_Angeles";
    var creationAddress = new AddressModel("1234 Main St", "Suite 100", "Los Angeles", "CA", "12345", "US");

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Branches", new BranchModificationModel(creationBranchName, creationTimeZoneId, creationAddress));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse1 = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var response1Model =
      await retrievalResponse1.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();
    response1Model.ShouldNotBeNull();
    var creationBranch = response1Model.Items.Single(x => string.Equals(x.Name, creationBranchName, StringComparison.OrdinalIgnoreCase));

    const string modificationBranchName = "Branch-509762905";
    const string modificationTimeZoneId = "America/New_York";
    var modificationAddress = new AddressModel("5234 Main St", "Suite 200", "New York", "NY", "62345", "US");

    var modificationResponse = await client.PutAsJsonAsync($"/api/1.0/Branches/{creationBranch?.Id}", new BranchModificationModel(modificationBranchName, modificationTimeZoneId, modificationAddress));

    modificationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse2 = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");

    var response2Model =
      await retrievalResponse2.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();

    response2Model.ShouldNotBeNull();
    response2Model.Items.Select(x => x.Name).ShouldContain(modificationBranchName);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).TimeZoneId.ShouldBe(modificationTimeZoneId);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address.ShouldNotBeNull();
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address?.Line1.ShouldBe(modificationAddress.Line1);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address?.Line2.ShouldBe(modificationAddress.Line2);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address?.City.ShouldBe(modificationAddress.City);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address?.Subdivision.ShouldBe(modificationAddress.Subdivision);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address?.PostalCode.ShouldBe(modificationAddress.PostalCode);
    response2Model.Items.Single(x => string.Equals(x.Name, modificationBranchName, StringComparison.OrdinalIgnoreCase)).Address?.CountryCode.ShouldBe(modificationAddress.CountryCode);
  }
}
