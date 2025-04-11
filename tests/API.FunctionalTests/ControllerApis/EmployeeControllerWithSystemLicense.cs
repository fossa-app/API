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

  public EmployeeControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory,
    ITestOutputHelper testOutputHelper)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithInvalidBranchId_ReturnsUnprocessableEntityAsync()
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

    // Try to assign to non-existent branch and non-existent department
    var response = await client.PutAsJsonAsync(
      $"/api/1.0/Employees/{employeeModel.Id}",
      new EmployeeManagementModel(AssignedBranchId: 999999, AssignedDepartmentId: 888888));

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithInvalidEmployeeId_ReturnsNotFoundAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var response = await client.PutAsJsonAsync(
      "/api/1.0/Employees/999999",
      new EmployeeManagementModel(AssignedBranchId: 1, AssignedDepartmentId: 2));

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithoutAccessToken_ReturnsUnauthorizedAsync()
  {
    var client = _factory.CreateClient();

    var response = await client.PutAsJsonAsync(
      "/api/1.0/Employees/1",
      new EmployeeManagementModel(AssignedBranchId: 1, AssignedDepartmentId: 2));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithUserAccessToken_ReturnsForbiddenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");

    var response = await client.PutAsJsonAsync(
      "/api/1.0/Employees/1",
      new EmployeeManagementModel(AssignedBranchId: 1, AssignedDepartmentId: 2));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task AssignEmployeeToBranch_WithValidData_SucceedsAsync()
  {
    // Create a branch first
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    var employee = await client.GetAsync("/api/1.0/Employee");
    var employeeModel = await employee.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeModel.ShouldNotBeNull();

    var branchResponse = await client.PostAsJsonAsync("/api/1.0/Branches",
        new BranchModificationModel("Test Branch", "America/New_York", null));
    branchResponse.EnsureSuccessStatusCode();

    var branchesResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");
    var branches = await branchesResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();
    branches.ShouldNotBeNull();
    var branch = branches.Items.FirstOrDefault(x => x.Name == "Test Branch");
    branch.ShouldNotBeNull("Branch not found");

    var departmentResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
        new DepartmentModificationModel("Test Department", null, employeeModel.Id));
    departmentResponse.EnsureSuccessStatusCode();

    var departmentsResponse = await client.GetAsync("/api/1.0/Departments?pageNumber=1&pageSize=100");
    var departments = await departmentsResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();
    departments.ShouldNotBeNull();
    var department = departments.Items.FirstOrDefault(x => x.Name == "Test Department");
    department.ShouldNotBeNull("Department not found");

    // Assign employee to branch
    var response = await client.PutAsync($"/api/1.0/Employees/{employeeModel.Id}",
        JsonContent.Create(new EmployeeManagementModel(branch.Id, department.Id)));

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  [Theory]
  [InlineData("", "Doe", "John Doe")]      // Empty first name
  [InlineData("John", "", "John Doe")]      // Empty last name
  [InlineData("John", "Doe", "")]          // Empty full name
  [InlineData(" ", "Doe", "John Doe")]     // Whitespace first name
  [InlineData("John", " ", "John Doe")]     // Whitespace last name
  [InlineData("John", "Doe", " ")]         // Whitespace full name
  public async Task CreateEmployee_WithInvalidData_ReturnsBadRequestAsync(string firstName, string lastName, string fullName)
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    var response = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel(firstName, lastName, fullName));

    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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
  public async Task DeleteEmployee_WithAssignedBranch_ReturnsPreconditionFailedAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    // Create a branch
    var branchResponse = await client.PostAsJsonAsync("/api/1.0/Branches",
        new BranchModificationModel("Test Branch", "America/New_York", null));
    branchResponse.EnsureSuccessStatusCode();

    var branchesResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");
    var branches = await branchesResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();
    branches.ShouldNotBeNull();
    var branch = branches.Items.FirstOrDefault(x => x.Name == "Test Branch");
    branch.ShouldNotBeNull("Branch not found");

    // Create an employee
    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    var currentEmployee = await client.GetAsync("/api/1.0/Employee");
    var employeeModel = await currentEmployee.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeModel.ShouldNotBeNull();

    var departmentResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
    new DepartmentModificationModel("Test Department", null, employeeModel.Id));
    departmentResponse.EnsureSuccessStatusCode();

    var departmentsResponse = await client.GetAsync("/api/1.0/Departments?pageNumber=1&pageSize=100");
    var departments = await departmentsResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();
    departments.ShouldNotBeNull();
    var department = departments.Items.FirstOrDefault(x => x.Name == "Test Department");
    department.ShouldNotBeNull("Department not found");

    // Assign employee to branch
    var assignResponse = await client.PutAsync($"/api/1.0/Employees/{employeeModel.Id}",
        JsonContent.Create(new EmployeeManagementModel(branch.Id, department.Id)));
    assignResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Try to delete the employee
    var deleteResponse = await client.DeleteAsync("/api/1.0/Employee");
    deleteResponse.StatusCode.ShouldBe(HttpStatusCode.PreconditionFailed);
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
  public async Task GetEmployee_FromDifferentTenant_ReturnsForbiddenAsync()
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
  public async Task GetEmployee_WithInvalidId_ReturnsNotFoundAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9ZHGBMC1DVP4R33QRYZ04RX.Tenant1.User1");

    var response = await client.GetAsync("/api/1.0/Employees/999999");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetEmployee_WithoutAccessToken_ReturnsUnauthorizedAsync()
  {
    var client = _factory.CreateClient();

    var response = await client.GetAsync("/api/1.0/Employees/1");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task GetEmployee_WithValidId_ReturnsEmployeeAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    // Create an employee first
    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    // Get employee ID from the current employee endpoint
    var currentEmployee = await client.GetAsync("/api/1.0/Employee");
    var employeeModel = await currentEmployee.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeModel.ShouldNotBeNull();

    // Get specific employee
    var response = await client.GetAsync($"/api/1.0/Employees/{employeeModel.Id}");
    response.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievedEmployee = await response.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    retrievedEmployee.ShouldNotBeNull();
    retrievedEmployee.FirstName.ShouldBe("John");
    retrievedEmployee.LastName.ShouldBe("Doe");
    retrievedEmployee.FullName.ShouldBe("John Doe");
  }

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
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
  public async Task UpdateBranch_WithValidData_SucceedsAsync()
  {
    // Create a branch first
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    var createResponse = await client.PostAsJsonAsync("/api/1.0/Branches",
        new BranchModificationModel("Test Branch", "America/New_York", null));
    createResponse.EnsureSuccessStatusCode();

    // Get the branch ID
    var branchesResponse = await client.GetAsync("/api/1.0/Branches?pageNumber=1&pageSize=100");
    var branches = await branchesResponse.Content.ReadFromJsonAsync<PagingResponseModel<BranchRetrievalModel>>();
    branches.ShouldNotBeNull();
    var branch = branches.Items.FirstOrDefault(x => x.Name == "Test Branch");
    branch.ShouldNotBeNull("Branch should exist");

    // Update the branch
    var updateResponse = await client.PutAsync($"/api/1.0/Branches/{branch.Id}",
        JsonContent.Create(new BranchModificationModel("Updated Branch", "America/Chicago",
            new AddressModel("123 Main St", null, "Chicago", "IL", "60601", "US"))));

    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Verify the update
    var getResponse = await client.GetAsync($"/api/1.0/Branches/{branch.Id}");
    var updatedBranch = await getResponse.Content.ReadFromJsonAsync<BranchRetrievalModel>();

    updatedBranch.ShouldNotBeNull();
    updatedBranch.Name.ShouldBe("Updated Branch");
    updatedBranch.TimeZoneId.ShouldBe("America/Chicago");
    updatedBranch.Address.ShouldNotBeNull();
    updatedBranch.Address.City.ShouldBe("Chicago");
  }

  [Fact]
  public async Task UpdateEmployee_WithValidData_SucceedsAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

    // Create an employee
    var createResponse = await client.PostAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("John", "Doe", "John Doe"));
    createResponse.EnsureSuccessStatusCode();

    // Update the employee
    var updateResponse = await client.PutAsJsonAsync("/api/1.0/Employee",
        new EmployeeModificationModel("Jane", "Smith", "Jane Smith"));
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Verify the update
    var getResponse = await client.GetAsync("/api/1.0/Employee");
    var updatedEmployee = await getResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();

    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.FirstName.ShouldBe("Jane");
    updatedEmployee.LastName.ShouldBe("Smith");
    updatedEmployee.FullName.ShouldBe("Jane Smith");
  }
}
