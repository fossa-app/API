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

      if (employeeCreationResponse.StatusCode != HttpStatusCode.OK)
      {
        _testOutputHelper.WriteLine(await employeeCreationResponse.Content.ReadAsStringAsync());
      }
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

  [Fact]
  public async Task UpdateEmployeeAssignedDepartmentAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425736");

    var employeeResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel("First", "Last", "Full Name"));
    employeeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var getEmployeeResponse = await client.GetAsync("/api/1.0/Employee");
    var employee = await getEmployeeResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employee.ShouldNotBeNull();

    var dept1Response = await client.PostAsJsonAsync("/api/1.0/Departments", new DepartmentModificationModel("Dept1", null, employee.Id));
    dept1Response.StatusCode.ShouldBe(HttpStatusCode.OK);
    var dept2Response = await client.PostAsJsonAsync("/api/1.0/Departments", new DepartmentModificationModel("Dept2", null, employee.Id));
    dept2Response.StatusCode.ShouldBe(HttpStatusCode.OK);

    var deptsResponse = await client.GetAsync("/api/1.0/Departments?pageNumber=1&pageSize=10");
    var depts = await deptsResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();
    depts.ShouldNotBeNull();
    var dept1Id = depts.Items.First(d => d.Name == "Dept1").Id;
    var dept2Id = depts.Items.First(d => d.Name == "Dept2").Id;

    var manageResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee.Id}", new EmployeeManagementModel(null, dept1Id, null));
    manageResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Act
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee.Id}", new EmployeeManagementModel(null, dept2Id, null));
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Assert
    var verifyResponse = await client.GetAsync($"/api/1.0/Employees/{employee.Id}");
    verifyResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var updatedEmployee = await verifyResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.AssignedDepartmentId.ShouldBe(dept2Id);
  }

  [Fact]
  public async Task UpdateEmployee_WithReportsTo_SucceedsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var manager = await CreateEmployeeAsync(client, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425737");
    var employee = await CreateEmployeeAsync(client, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425738");

    // Act
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee.Id}", new EmployeeManagementModel(null, null, manager.Id));
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Assert
    var verifyResponse = await client.GetAsync($"/api/1.0/Employees/{employee.Id}");
    verifyResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var updatedEmployee = await verifyResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.ReportsToId.ShouldBe(manager.Id);
  }

  [Fact]
  public async Task DeleteEmployee_WithDirectReport_FailsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var manager = await CreateEmployeeAsync(client, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425739");
    var employee = await CreateEmployeeAsync(client, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425740");
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee.Id}", new EmployeeManagementModel(null, null, manager.Id));
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Act
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425739");
    var deleteResponse = await client.DeleteAsync("/api/1.0/Employee");

    // Assert
    deleteResponse.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task UpdateEmployee_WithNonExistentReportsTo_FailsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var employee = await CreateEmployeeAsync(client, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425741");

    // Act
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee.Id}", new EmployeeManagementModel(null, null, 999999L));

    // Assert
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateEmployee_WithCyclicalReference_FailsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var employee1 = await CreateEmployeeAsync(client, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425742");
    var employee2 = await CreateEmployeeAsync(client, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425743");

    await client.PutAsJsonAsync($"/api/1.0/Employees/{employee2.Id}", new EmployeeManagementModel(null, null, employee1.Id));

    // Act
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee1.Id}", new EmployeeManagementModel(null, null, employee2.Id));

    // Assert
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateEmployee_WithLongerCyclicalReference_FailsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var employee1 = await CreateEmployeeAsync(client, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425744");
    var employee2 = await CreateEmployeeAsync(client, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425745");
    var employee3 = await CreateEmployeeAsync(client, "Employee", "Three", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425746");

    await client.PutAsJsonAsync($"/api/1.0/Employees/{employee2.Id}", new EmployeeManagementModel(null, null, employee1.Id));
    await client.PutAsJsonAsync($"/api/1.0/Employees/{employee3.Id}", new EmployeeManagementModel(null, null, employee2.Id));

    // Act
    var updateResponse = await client.PutAsJsonAsync($"/api/1.0/Employees/{employee1.Id}", new EmployeeManagementModel(null, null, employee3.Id));

    // Assert
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task ListEmployees_ByReportsToId_ReturnsFilteredResultsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var manager = await CreateEmployeeAsync(client, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425747");
    var employee1 = await CreateEmployeeAsync(client, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425748");
    var employee2 = await CreateEmployeeAsync(client, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425749");
    await CreateEmployeeAsync(client, "Employee", "Three", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425750");

    await client.PutAsJsonAsync($"/api/1.0/Employees/{employee1.Id}", new EmployeeManagementModel(null, null, manager.Id));
    await client.PutAsJsonAsync($"/api/1.0/Employees/{employee2.Id}", new EmployeeManagementModel(null, null, manager.Id));

    // Act
    var response = await client.GetAsync($"/api/1.0/Employees?reportsToId={manager.Id}&pageNumber=1&pageSize=10");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<PagingResponseModel<EmployeeRetrievalModel>>();
    result.ShouldNotBeNull();
    result.Items.Count.ShouldBe(2);
    result.Items.All(x => x.ReportsToId == manager.Id).ShouldBeTrue();
  }

  [Fact]
  public async Task ListEmployees_TopLevelOnly_ReturnsFilteredResultsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    var manager = await CreateEmployeeAsync(client, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425751");
    var employee = await CreateEmployeeAsync(client, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425752");
    await client.PutAsJsonAsync($"/api/1.0/Employees/{employee.Id}", new EmployeeManagementModel(null, null, manager.Id));

    // Act
    var response = await client.GetAsync("/api/1.0/Employees?topLevelOnly=true&pageNumber=1&pageSize=10");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<PagingResponseModel<EmployeeRetrievalModel>>();
    result.ShouldNotBeNull();
    result.Items.All(x => x.ReportsToId == null).ShouldBeTrue();
    result.Items.Any(x => x.Id == employee.Id).ShouldBeFalse();
    result.Items.Any(x => x.Id == manager.Id).ShouldBeTrue();
  }

  private async Task<EmployeeRetrievalModel> CreateEmployeeAsync(HttpClient client, string firstName, string lastName, string? token = null)
  {
    if (token != null)
    {
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, $"{firstName} {lastName}"));
    if (creationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await creationResponse.Content.ReadAsStringAsync());
    }
    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Employee");
    var employee = await retrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employee.ShouldNotBeNull();
    return employee;
  }
}
