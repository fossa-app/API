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

    (await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateExistingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1");

    const string firstName = "First812685875";
    const string lastName = "Last812685875";
    const string fullName = "Full812685875";

    (await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Conflict);

    var responseModel = (await employeeClient.getEmployeesAsync(new EmployeeQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.items.ShouldNotBeEmpty();
    var employee = responseModel.items.First();
    employee.id.ShouldBePositive();
    employee.firstName.ShouldNotBe(firstName);
    employee.lastName.ShouldNotBe(lastName);
    employee.fullName.ShouldNotBe(fullName);
  }

  [Fact]
  public async Task CreateMissingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User100");

    const string firstName = "First576536102";
    const string lastName = "Last576536102";
    const string fullName = "Full576536102";

    await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var responseModel = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();
    responseModel.id.ShouldBePositive();
    responseModel.firstName.ShouldBe(firstName);
    responseModel.lastName.ShouldBe(lastName);
    responseModel.fullName.ShouldBe(fullName);
  }

  [Fact]
  public async Task DeleteEmployeeWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;

    (await employeeClient.deleteCurrentEmployeeAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized); // Auth will fail first
  }

  [Fact]
  public async Task DeleteExistingEmployeeWithoutDependenciesWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    var employeeEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<EmployeeMongoEntity, long>();

    accessTokenContext.SetAccessToken("01J9ZHFMA4ZXFE4EVMZARRTK7M.Tenant2.User1");

    var existingEmployee = employeeEasyStore.Entities.Values.Single(x => string.Equals(x.FullName, "Meaghan Riley", StringComparison.OrdinalIgnoreCase));

    await employeeClient.deleteCurrentEmployeeAsync(TestContext.Current.CancellationToken);

    employeeEasyStore.Entities.ContainsKey(existingEmployee.ID).ShouldBeFalse();
  }

  [Fact]
  public async Task DeleteMissingEmployeeWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01J9ZHG6056FSRFJ7GC9E21JFD.Tenant1000.User1");

    // Attempting to delete the authenticated user's employee explicitly is idempotent and succeeds
    await Should.NotThrowAsync(async () => await employeeClient.deleteCurrentEmployeeAsync(TestContext.Current.CancellationToken));
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
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    var employeeIds = new List<long>();

    foreach (var x in Seq(36883136, 36883144, 36883148))
    {
      accessTokenContext.SetAccessToken($"01JND081MJRM7Q0CHWEY1038EF.Tenant1.User{x}");

      var firstName = $"First{x}";
      var lastName = $"Last{x}";
      var fullName = $"Full{x}";

      await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

      var employeeResponseModel = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();
      employeeIds.Add(employeeResponseModel.id);
    }

    var employee1Id = employeeIds[0];
    var employee2Id = employeeIds[1];
    var employee3Id = employeeIds[2];
    const int employee4Id = 204298046; // Missing employee

    // Act
    var branchResponseModel = (await employeeClient.getEmployeesAsync(new EmployeeQueryRequestModel { id = [employee1Id, employee2Id, employee3Id, employee4Id] }, TestContext.Current.CancellationToken)).Unwrap();

    // Assert

    branchResponseModel.ShouldNotBeNull();

    branchResponseModel.pageNumber.ShouldBeNull();
    branchResponseModel.pageSize.ShouldBeNull();
    branchResponseModel.totalItems.ShouldBeNull();
    branchResponseModel.totalPages.ShouldBeNull();
    branchResponseModel.items.Count.ShouldBe(3);
    branchResponseModel.items.ShouldContain(x => x.id == employee1Id);
    branchResponseModel.items.ShouldContain(x => x.id == employee2Id);
    branchResponseModel.items.ShouldContain(x => x.id == employee3Id);
    branchResponseModel.items.ShouldNotContain(x => x.id == employee4Id);
  }

  [Fact]
  public async Task RetrieveEmployeeWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    accessTokenContext.SetAccessToken("01J9ZHGBMC1DVP4R33QRYZ04RX.Tenant1.User1");

    var responseModel = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();

    responseModel.ShouldNotBeNull();

    responseModel.id.ShouldBePositive();
    responseModel.companyId.ShouldBePositive();
    responseModel.firstName.ShouldNotBeNullOrEmpty();
    responseModel.lastName.ShouldNotBeNullOrEmpty();
    responseModel.fullName.ShouldNotBeNullOrEmpty();
  }

  [Fact]
  public async Task RetrieveMissingEmployeeWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();
    accessTokenContext.SetAccessToken("01J9ZHGFXBG4WKNJKBSG7T2Y76.Tenant1000.User1000");

    // The endpoint returns 404 if the authenticated user's employee cannot be found
    (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UpdateEmployeeAssignedDepartmentAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var departmentClient = scope.ServiceProvider.GetRequiredService<IClients>().DepartmentClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425736");

    await employeeClient.createEmployeeAsync(new EmployeeModificationModel("First", "Last", "Full Name"), TestContext.Current.CancellationToken);

    var employee = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();
    employee.ShouldNotBeNull();

    await departmentClient.createDepartmentAsync(new DepartmentModificationModel("Dept1", null, employee.id), TestContext.Current.CancellationToken);
    await departmentClient.createDepartmentAsync(new DepartmentModificationModel("Dept2", null, employee.id), TestContext.Current.CancellationToken);

    var depts = (await departmentClient.getDepartmentsAsync(new DepartmentQueryRequestModel { pageNumber = 1, pageSize = 10 }, TestContext.Current.CancellationToken)).Unwrap();
    depts.ShouldNotBeNull();
    var dept1Id = depts.items.First(d => d.name == "Dept1").id;
    var dept2Id = depts.items.First(d => d.name == "Dept2").id;

    await employeeClient.manageEmployeeAsync(employee.id, new EmployeeManagementModel(null, dept1Id, null, "Staff"), TestContext.Current.CancellationToken);

    // Act
    await employeeClient.manageEmployeeAsync(employee.id, new EmployeeManagementModel(null, dept2Id, null, "Staff"), TestContext.Current.CancellationToken);

    // Assert
    var updatedEmployee = (await employeeClient.getEmployeeAsync(employee.id, TestContext.Current.CancellationToken)).Unwrap();
    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.assignedDepartmentId.ShouldBe(dept2Id);
  }

  [Fact]
  public async Task UpdateEmployee_WithReportsTo_SucceedsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var manager = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425737");
    var employee = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425738");

    // Act
    await employeeClient.manageEmployeeAsync(employee.id, new EmployeeManagementModel(null, null, manager.id, "Staff"), TestContext.Current.CancellationToken);

    // Assert
    var updatedEmployee = (await employeeClient.getEmployeeAsync(employee.id, TestContext.Current.CancellationToken)).Unwrap();
    updatedEmployee.ShouldNotBeNull();
    updatedEmployee.reportsToId.ShouldBe(manager.id);
  }

  [Fact]
  public async Task DeleteEmployee_WithDirectReport_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var manager = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425739");
    var employee = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425740");
    await employeeClient.manageEmployeeAsync(employee.id, new EmployeeManagementModel(null, null, manager.id, "Staff"), TestContext.Current.CancellationToken);

    // Act & Assert
    accessTokenContext.SetAccessToken("01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425739");

    (await employeeClient.deleteCurrentEmployeeAsync(TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task UpdateEmployee_WithNonExistentReportsTo_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var employee = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425741");

    // Act
    (await employeeClient.manageEmployeeAsync(employee.id, new EmployeeManagementModel(null, null, 999999L, "Staff"), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateEmployee_WithCyclicalReference_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var employee1 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425742");
    var employee2 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425743");

    await employeeClient.manageEmployeeAsync(employee2.id, new EmployeeManagementModel(null, null, employee1.id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    (await employeeClient.manageEmployeeAsync(employee1.id, new EmployeeManagementModel(null, null, employee2.id, "Staff"), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateEmployee_WithLongerCyclicalReference_FailsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var employee1 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425744");
    var employee2 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425745");
    var employee3 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "Three", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425746");

    await employeeClient.manageEmployeeAsync(employee2.id, new EmployeeManagementModel(null, null, employee1.id, "Staff"), TestContext.Current.CancellationToken);
    await employeeClient.manageEmployeeAsync(employee3.id, new EmployeeManagementModel(null, null, employee2.id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    (await employeeClient.manageEmployeeAsync(employee1.id, new EmployeeManagementModel(null, null, employee3.id, "Staff"), TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task ListEmployees_ByReportsToId_ReturnsFilteredResultsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var manager = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425747");
    var employee1 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "One", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425748");
    var employee2 = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "Two", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425749");
    await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "Three", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425750");

    await employeeClient.manageEmployeeAsync(employee1.id, new EmployeeManagementModel(null, null, manager.id, "Staff"), TestContext.Current.CancellationToken);
    await employeeClient.manageEmployeeAsync(employee2.id, new EmployeeManagementModel(null, null, manager.id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    var result = (await employeeClient.getEmployeesAsync(new EmployeeQueryRequestModel
    {
      reportsToId = manager.id,
      pageNumber = 1,
      pageSize = 10
    }, TestContext.Current.CancellationToken)).Unwrap();

    // Assert
    result.ShouldNotBeNull();
    result.items.Count.ShouldBe(2);
    result.items.All(x => x.reportsToId == manager.id).ShouldBeTrue();
  }

  [Fact]
  public async Task ListEmployees_TopLevelOnly_ReturnsFilteredResultsAsync()
  {
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    var manager = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Manager", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425751");
    var employee = await CreateEmployeeAsync(employeeClient, accessTokenContext, "Employee", "User", "01K4H70V6A2K39JB4NCYPQ07KY.Tenant1.ADMIN420425752");
    await employeeClient.manageEmployeeAsync(employee.id, new EmployeeManagementModel(null, null, manager.id, "Staff"), TestContext.Current.CancellationToken);

    // Act
    var result = (await employeeClient.getEmployeesAsync(new EmployeeQueryRequestModel
    {
      topLevelOnly = true,
      pageNumber = 1,
      pageSize = 10
    }, TestContext.Current.CancellationToken)).Unwrap();

    // Assert
    result.ShouldNotBeNull();
    result.items.All(x => x.reportsToId == null).ShouldBeTrue();
    result.items.Any(x => x.id == employee.id).ShouldBeFalse();
    result.items.Any(x => x.id == manager.id).ShouldBeTrue();
  }

  private static async Task<EmployeeRetrievalModel> CreateEmployeeAsync(IEmployeeClient client, IAccessTokenContext accessTokenContext, string firstName, string lastName, string? token = null)
  {
    if (token != null)
    {
      accessTokenContext.SetAccessToken(token);
    }

    await client.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, $"{firstName} {lastName}"), TestContext.Current.CancellationToken);

    return (await client.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();
  }
}
