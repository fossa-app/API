﻿using System.Net;
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

public class EmployeeControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public EmployeeControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory,
    ITestOutputHelper testOutputHelper)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithInvalidBranchId_ReturnsUnprocessableEntity()
  {
    // First create an employee
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    var employee = await client.GetAsync("/api/1.0/Employee");
    var employeeModel = await employee.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeModel.ShouldNotBeNull();

    // Try to assign to non-existent branch
    var response = await client.PutAsJsonAsync(
      $"/api/1.0/Employees/{employeeModel.Id}",
      new EmployeeManagementModel(AssignedBranchId: 999999));

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithInvalidEmployeeId_ReturnsNotFound()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var response = await client.PutAsJsonAsync(
      "/api/1.0/Employees/999999",
      new EmployeeManagementModel(AssignedBranchId: 1));

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithoutAccessToken_ReturnsUnauthorized()
  {
    var client = _factory.CreateClient();

    var response = await client.PutAsJsonAsync(
      "/api/1.0/Employees/1",
      new EmployeeManagementModel(AssignedBranchId: 1));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithUserAccessToken_ReturnsForbidden()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");

    var response = await client.PutAsJsonAsync(
      "/api/1.0/Employees/1",
      new EmployeeManagementModel(AssignedBranchId: 1));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithValidData_Succeeds()
  {
    // Create a branch first
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var branchResponse = await client.PostAsJsonAsync("/api/1.0/Branches",
        new BranchModificationModel("Test Branch", "America/New_York", null));
    branchResponse.EnsureSuccessStatusCode();

    var branchesResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");
    var branches = await branchesResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();
    branches.ShouldNotBeNull();
    var branch = branches.Items.First(x => x.Name == "Test Branch");

    // Create an employee
    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    var employee = await client.GetAsync("/api/1.0/Employee");
    var employeeModel = await employee.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeModel.ShouldNotBeNull();

    // Assign employee to branch
    var response = await client.PutAsync($"/api/1.0/Employees/{employeeModel.Id}",
        JsonContent.Create(new EmployeeManagementModel(branch.Id)));

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  [Fact]
  public async Task CreateEmployeeWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();

    const string firstName = "First576536102";
    const string lastName = "Last576536102";
    const string fullName = "Full576536102";

    var response = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateExistingEmployeeWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1");

    const string firstName = "First812685875";
    const string lastName = "Last812685875";
    const string fullName = "Full812685875";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);

    var retrievalResponse = await client.GetAsync("/api/1.0/Employee");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.FirstName.ShouldNotBe(firstName);
    responseModel.LastName.ShouldNotBe(lastName);
    responseModel.FullName.ShouldNotBe(fullName);
  }

  [Fact]
  public async Task CreateMissingEmployeeWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User100");

    const string firstName = "First576536102";
    const string lastName = "Last576536102";
    const string fullName = "Full576536102";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Employee");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.FirstName.ShouldBe(firstName);
    responseModel.LastName.ShouldBe(lastName);
    responseModel.FullName.ShouldBe(fullName);
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
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHFMA4ZXFE4EVMZARRTK7M.Tenant2.User1");

    var existingEmployee = employeeEasyStore.Entities.Values.Single(x => string.Equals(x.FullName, "Meaghan Riley", StringComparison.OrdinalIgnoreCase));

    var response = await client.DeleteAsync("/api/1.0/Employee");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    employeeEasyStore.Entities.ContainsKey(existingEmployee.ID).ShouldBeFalse();
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

  [Fact]
  public async Task GetEmployee_FromDifferentTenant_ReturnsForbidden()
  {
    // First create an employee in tenant 1
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    var employee = await client.GetAsync("/api/1.0/Employee");
    var employeeModel = await employee.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeModel.ShouldNotBeNull();

    // Try to access the employee from tenant 2
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant2.User1");

    var response = await client.GetAsync($"/api/1.0/Employees/{employeeModel.Id}");

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task GetEmployee_WithInvalidId_ReturnsNotFound()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHGBMC1DVP4R33QRYZ04RX.Tenant1.User1");

    var response = await client.GetAsync("/api/1.0/Employees/999999");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetEmployee_WithoutAccessToken_ReturnsUnauthorized()
  {
    var client = _factory.CreateClient();

    var response = await client.GetAsync("/api/1.0/Employees/1");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task ListCreatedEmployeesAsync()
  {
    // Arrange

    var client = _factory.CreateClient();
    var employeeIds = new List<long>();

    foreach (var x in Seq(36883136, 36883144, 36883148))
    {
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"01JND081MJRM7Q0CHWEY1038EF.Tenant1.User{x}");

      var firstName = $"First{x}";
      var lastName = $"Last{x}";
      var fullName = $"Full{x}";

      var employeeCreationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));

      employeeCreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

      var employeeRetrievalResponse = await client.GetAsync("/api/1.0/Employee");

      var employeeResponseModel =
        await employeeRetrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();

      employeeIds.Add(employeeResponseModel?.Id ?? 0);
    }

    var employee1Id = employeeIds[0];
    var employee2Id = employeeIds[1];
    var employee3Id = employeeIds[2];
    const int employee4Id = 204298046; // Missing employee

    // Act

    var branchRetrievalResponse = await client.GetAsync($"/api/1.0/Employees?Id={employee1Id}&Id={employee2Id}&Id={employee3Id}&Id={employee4Id}");

    if (branchRetrievalResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await branchRetrievalResponse.Content.ReadAsStringAsync());
    }
    branchRetrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var branchResponseModel =
      await branchRetrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<EmployeeRetrievalModel>>();

    // Assert

    branchResponseModel.ShouldNotBeNull();

    branchResponseModel.PageNumber.ShouldBeNull();
    branchResponseModel.PageSize.ShouldBeNull();
    branchResponseModel.TotalItems.ShouldBeNull();
    branchResponseModel.TotalPages.ShouldBeNull();
    branchResponseModel.Items.Count.ShouldBe(3);
    branchResponseModel.Items.ShouldContain(x => x.Id == employee1Id);
    branchResponseModel.Items.ShouldContain(x => x.Id == employee2Id);
    branchResponseModel.Items.ShouldContain(x => x.Id == employee3Id);
    branchResponseModel.Items.ShouldNotContain(x => x.Id == employee4Id);
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

    response.StatusCode.ShouldBe(HttpStatusCode.OK);

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
