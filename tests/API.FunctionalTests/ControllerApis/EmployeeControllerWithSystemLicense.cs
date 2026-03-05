using System.Net;
using EasyDoubles;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web;
using Fossa.Bridge.Models.ApiModels;
using Fossa.Bridge.Services;
using Fossa.Bridge.Services.Clients;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class EmployeeControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public EmployeeControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateEmployeeWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;

    const string firstName = "First576536102";
    const string lastName = "Last576536102";
    const string fullName = "Full576536102";

    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateExistingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1");

    const string firstName = "First812685875";
    const string lastName = "Last812685875";
    const string fullName = "Full812685875";

    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Conflict);

    var responseModel = await employeeClient.GetEmployeesAsync(new EmployeeQueryRequestModel { PageNumber = 1, PageSize = 100 }, TestContext.Current.CancellationToken);

    responseModel.ShouldNotBeNull();
    responseModel.Items.ShouldNotBeEmpty();
    var employee = responseModel.Items.First();
    employee.Id.ShouldBePositive();
    employee.FirstName.ShouldNotBe(firstName);
    employee.LastName.ShouldNotBe(lastName);
    employee.FullName.ShouldNotBe(fullName);
  }

  [Fact]
  public async Task CreateMissingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User100");

    const string firstName = "First576536102";
    const string lastName = "Last576536102";
    const string fullName = "Full576536102";

    await employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var responseModel = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.FirstName.ShouldBe(firstName);
    responseModel.LastName.ShouldBe(lastName);
    responseModel.FullName.ShouldBe(fullName);
  }

  [Fact]
  public async Task DeleteEmployeeWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;

    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.DeleteCurrentEmployeeAsync(TestContext.Current.CancellationToken)); // Auth will fail first

    ex.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingEmployeeWithoutDependenciesWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    var employeeEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<EmployeeMongoEntity, long>();

    transport.SetAuthorizationToken("Bearer", "01J9ZHFMA4ZXFE4EVMZARRTK7M.Tenant2.User1");

    var existingEmployee = employeeEasyStore.Entities.Values.Single(x => string.Equals(x.FullName, "Meaghan Riley", StringComparison.OrdinalIgnoreCase));

    await employeeClient.DeleteCurrentEmployeeAsync(TestContext.Current.CancellationToken);

    employeeEasyStore.Entities.ContainsKey(existingEmployee.ID).ShouldBeFalse();
  }

  [Fact]
  public async Task DeleteMissingEmployeeWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01J9ZHG6056FSRFJ7GC9E21JFD.Tenant1000.User1");

    // Attempting to delete the authenticated user's employee explicitly is idempotent and succeeds
    await Should.NotThrowAsync(async () => await employeeClient.DeleteCurrentEmployeeAsync(TestContext.Current.CancellationToken));
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  [Fact]
  public async Task ListCreatedEmployeesAsync()
  {
    // Arrange

    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    var employeeIds = new List<long>();

    foreach (var x in Seq(36883136, 36883144, 36883148))
    {
      transport.SetAuthorizationToken("Bearer", $"01JND081MJRM7Q0CHWEY1038EF.Tenant1.User{x}");

      var firstName = $"First{x}";
      var lastName = $"Last{x}";
      var fullName = $"Full{x}";

      await employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

      var employeeResponseModel = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);
      employeeIds.Add(employeeResponseModel?.Id ?? 0);
    }

    var employee1Id = employeeIds[0];
    var employee2Id = employeeIds[1];
    var employee3Id = employeeIds[2];
    const int employee4Id = 204298046; // Missing employee

    // Act
    var branchResponseModel = await employeeClient.GetEmployeesAsync(new EmployeeQueryRequestModel { Id = [employee1Id, employee2Id, employee3Id, employee4Id] }, TestContext.Current.CancellationToken);

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
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    transport.SetAuthorizationToken("Bearer", "01J9ZHGBMC1DVP4R33QRYZ04RX.Tenant1.User1");

    var responseModel = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);

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
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    transport.SetAuthorizationToken("Bearer", "01J9ZHGFXBG4WKNJKBSG7T2Y76.Tenant1000.User1000");

    // The endpoint returns 404 if the authenticated user's employee cannot be found
    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UpdateEmployeeAssignedDepartmentAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var departmentClient = scope.ServiceProvider.GetRequiredService<IClients>().DepartmentClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425736");

    await employeeClient.CreateEmployeeAsync(new EmployeeModificationModel("First", "Last", "Full Name"), TestContext.Current.CancellationToken);

    var employee = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);
    employee.ShouldNotBeNull();

    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel("Dept1", null, employee.Id), TestContext.Current.CancellationToken);
    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel("Dept2", null, employee.Id), TestContext.Current.CancellationToken);

    var depts = await departmentClient.GetDepartmentsAsync(new DepartmentQueryRequestModel { PageNumber = 1, PageSize = 10 }, TestContext.Current.CancellationToken);
    depts.ShouldNotBeNull();
    var dept1Id = depts.Items.First(d => d.Name == "Dept1").Id;
    var dept2Id = depts.Items.First(d => d.Name == "Dept2").Id;

    await employeeClient.ManageEmployeeAsync(employee.Id, new EmployeeManagementModel(null, dept1Id, null, "Staff"), TestContext.Current.CancellationToken);

    // Act
    await employeeClient.ManageEmployeeAsync(employee.Id, new EmployeeManagementModel(null, dept2Id, null, "Staff"), TestContext.Current.CancellationToken);

    // Assert
    var updatedEmployee = await employeeClient.GetEmployeeAsync(employee.Id, TestContext.Current.CancellationToken);
    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.AssignedDepartmentId.ShouldBe(dept2Id);
  }

  [Fact]
  public async Task UpdateEmployee_WithReportsTo_SucceedsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var manager = await CreateEmployeeAsync(employeeClient, transport, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425737");
    var employee = await CreateEmployeeAsync(employeeClient, transport, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425738");

    // Act
    await employeeClient.ManageEmployeeAsync(employee.Id, new EmployeeManagementModel(null, null, manager.Id, "Staff"), TestContext.Current.CancellationToken);

    // Assert
    var updatedEmployee = await employeeClient.GetEmployeeAsync(employee.Id, TestContext.Current.CancellationToken);
    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.ReportsToId.ShouldBe(manager.Id);
  }

  [Fact]
  public async Task DeleteEmployee_WithDirectReport_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var manager = await CreateEmployeeAsync(employeeClient, transport, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425739");
    var employee = await CreateEmployeeAsync(employeeClient, transport, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425740");
    await employeeClient.ManageEmployeeAsync(employee.Id, new EmployeeManagementModel(null, null, manager.Id, "Staff"), TestContext.Current.CancellationToken);

    // Act & Assert
    transport.SetAuthorizationToken("Bearer", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425739");

    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.DeleteCurrentEmployeeAsync(TestContext.Current.CancellationToken));
    ex.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task UpdateEmployee_WithNonExistentReportsTo_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var employee = await CreateEmployeeAsync(employeeClient, transport, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425741");

    // Act
    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.ManageEmployeeAsync(employee.Id, new EmployeeManagementModel(null, null, 999999L, "Staff"), TestContext.Current.CancellationToken));

    // Assert
    ex.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateEmployee_WithCyclicalReference_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var employee1 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425742");
    var employee2 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425743");

    await employeeClient.ManageEmployeeAsync(employee2.Id, new EmployeeManagementModel(null, null, employee1.Id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.ManageEmployeeAsync(employee1.Id, new EmployeeManagementModel(null, null, employee2.Id, "Staff"), TestContext.Current.CancellationToken));

    // Assert
    ex.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateEmployee_WithLongerCyclicalReference_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var employee1 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425744");
    var employee2 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425745");
    var employee3 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "Three", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425746");

    await employeeClient.ManageEmployeeAsync(employee2.Id, new EmployeeManagementModel(null, null, employee1.Id, "Staff"), TestContext.Current.CancellationToken);
    await employeeClient.ManageEmployeeAsync(employee3.Id, new EmployeeManagementModel(null, null, employee2.Id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    var ex = await Should.ThrowAsync<HttpRequestException>(() => employeeClient.ManageEmployeeAsync(employee1.Id, new EmployeeManagementModel(null, null, employee3.Id, "Staff"), TestContext.Current.CancellationToken));

    // Assert
    ex.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task ListEmployees_ByReportsToId_ReturnsFilteredResultsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var manager = await CreateEmployeeAsync(employeeClient, transport, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425747");
    var employee1 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425748");
    var employee2 = await CreateEmployeeAsync(employeeClient, transport, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425749");
    await CreateEmployeeAsync(employeeClient, transport, "Employee", "Three", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425750");

    await employeeClient.ManageEmployeeAsync(employee1.Id, new EmployeeManagementModel(null, null, manager.Id, "Staff"), TestContext.Current.CancellationToken);
    await employeeClient.ManageEmployeeAsync(employee2.Id, new EmployeeManagementModel(null, null, manager.Id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    var result = await employeeClient.GetEmployeesAsync(new EmployeeQueryRequestModel
    {
      ReportsToId = manager.Id,
      PageNumber = 1,
      PageSize = 10
    }, TestContext.Current.CancellationToken);

    // Assert
    result.ShouldNotBeNull();
    result.Items.Count.ShouldBe(2);
    result.Items.All(x => x.ReportsToId == manager.Id).ShouldBeTrue();
  }

  [Fact]
  public async Task ListEmployees_TopLevelOnly_ReturnsFilteredResultsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    var manager = await CreateEmployeeAsync(employeeClient, transport, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425751");
    var employee = await CreateEmployeeAsync(employeeClient, transport, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425752");
    await employeeClient.ManageEmployeeAsync(employee.Id, new EmployeeManagementModel(null, null, manager.Id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    var result = await employeeClient.GetEmployeesAsync(new EmployeeQueryRequestModel
    {
      TopLevelOnly = true,
      PageNumber = 1,
      PageSize = 10
    }, TestContext.Current.CancellationToken);

    // Assert
    result.ShouldNotBeNull();
    result.Items.All(x => x.ReportsToId == null).ShouldBeTrue();
    result.Items.Any(x => x.Id == employee.Id).ShouldBeFalse();
    result.Items.Any(x => x.Id == manager.Id).ShouldBeTrue();
  }

  private static async Task<EmployeeRetrievalModel> CreateEmployeeAsync(IEmployeeClient client, TestHttpTransport transport, string firstName, string lastName, string? token = null)
  {
    if (token != null)
    {
      transport.SetAuthorizationToken("Bearer", token);
    }

    await client.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, $"{firstName} {lastName}"), TestContext.Current.CancellationToken);

    var employee = await client.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);
    return employee;
  }
}
