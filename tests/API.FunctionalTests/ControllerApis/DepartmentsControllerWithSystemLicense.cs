using System.Net;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.Bridge.Models.ApiModels;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class DepartmentsControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public DepartmentsControllerWithSystemLicense(
      CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory;
  }

  [Fact]
  public async Task CreateAndListDepartmentsAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var departmentClient = scope.ServiceProvider.GetRequiredService<IClients>().DepartmentClient;
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN420425736");

    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    var dept1Name = $"Department-{Random.Shared.Next()}";
    var dept2Name = $"Department-{Random.Shared.Next()}";
    var dept3Name = $"Department-{Random.Shared.Next()}";

    await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var createdEmployee = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();
    createdEmployee.ShouldNotBeNull();

    // Act - Create departments
    await departmentClient.createDepartmentAsync(new DepartmentModificationModel(dept1Name, null, createdEmployee.id), TestContext.Current.CancellationToken);
    await departmentClient.createDepartmentAsync(new DepartmentModificationModel(dept2Name, null, createdEmployee.id), TestContext.Current.CancellationToken);
    await departmentClient.createDepartmentAsync(new DepartmentModificationModel(dept3Name, null, createdEmployee.id), TestContext.Current.CancellationToken);

    // Get the created departments
    var retrievalResponseModel = (await departmentClient.getDepartmentsAsync(new DepartmentQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();

    retrievalResponseModel.ShouldNotBeNull();
    var dept1Model = retrievalResponseModel.items.Single(x => string.Equals(x.name, dept1Name, StringComparison.OrdinalIgnoreCase));
    var dept2Model = retrievalResponseModel.items.Single(x => string.Equals(x.name, dept2Name, StringComparison.OrdinalIgnoreCase));
    var dept3Model = retrievalResponseModel.items.Single(x => string.Equals(x.name, dept3Name, StringComparison.OrdinalIgnoreCase));

    var dept1Id = dept1Model.id;
    var dept2Id = dept2Model.id;
    var dept3Id = dept3Model.id;
    const int dept4Id = 204298046; // Missing department

    // Query specific departments
    var deptResponseModel = (await departmentClient.getDepartmentsAsync(new DepartmentQueryRequestModel { id = [dept1Id, dept2Id, dept3Id, dept4Id] }, TestContext.Current.CancellationToken)).Unwrap();

    deptResponseModel.ShouldNotBeNull();
    deptResponseModel.items.Count.ShouldBe(3);
    deptResponseModel.items.ShouldContain(x => x.id == dept1Id);
    deptResponseModel.items.ShouldContain(x => x.id == dept2Id);
    deptResponseModel.items.ShouldContain(x => x.id == dept3Id);
    deptResponseModel.items.ShouldNotContain(x => x.id == dept4Id);
  }

  [Fact]
  public async Task DeleteExistingDepartmentWithoutDependenciesWithAdministratorAccessTokenAsync()
  {
    // Arrange
    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    using var scope = _factory.Services.CreateScope();
    var departmentClient = scope.ServiceProvider.GetRequiredService<IClients>().DepartmentClient;
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN430851539");

    await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var createdEmployee = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();
    createdEmployee.ShouldNotBeNull();

    var departmentName = $"Department-{Random.Shared.Next()}";

    await departmentClient.createDepartmentAsync(new DepartmentModificationModel(departmentName, null, createdEmployee.id), TestContext.Current.CancellationToken);

    var retrievalResponseModel = (await departmentClient.getDepartmentsAsync(new DepartmentQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    var department = retrievalResponseModel.items.Single(x => string.Equals(x.name, departmentName, StringComparison.OrdinalIgnoreCase));

    // Act
    await departmentClient.deleteDepartmentAsync(department!.id, TestContext.Current.CancellationToken);

    // Assert
    (await departmentClient.getDepartmentAsync(department.id, TestContext.Current.CancellationToken)).ShouldFailWith(HttpStatusCode.NotFound);
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  [Fact]
  public async Task UpdateExistingDepartmentWithAdministratorAccessTokenAsync()
  {
    // Arrange
    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    using var scope = _factory.Services.CreateScope();
    var departmentClient = scope.ServiceProvider.GetRequiredService<IClients>().DepartmentClient;
    var employeeClient = scope.ServiceProvider.GetRequiredService<IClients>().EmployeeClient;
    var accessTokenContext = scope.ServiceProvider.GetRequiredService<IAccessTokenContext>();

    accessTokenContext.SetAccessToken("01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN604735919");

    await employeeClient.createEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var createdEmployee = (await employeeClient.getCurrentEmployeeAsync(TestContext.Current.CancellationToken)).Unwrap();
    createdEmployee.ShouldNotBeNull();

    var departmentName = $"Department-{Random.Shared.Next()}";
    await departmentClient.createDepartmentAsync(new DepartmentModificationModel(departmentName, null, createdEmployee.id), TestContext.Current.CancellationToken);

    var retrievalResponseModel = (await departmentClient.getDepartmentsAsync(new DepartmentQueryRequestModel { pageNumber = 1, pageSize = 100 }, TestContext.Current.CancellationToken)).Unwrap();
    var departmentId = retrievalResponseModel.items.Single(x => string.Equals(x.name, departmentName, StringComparison.OrdinalIgnoreCase)).id;

    // Act
    var updatedName = $"Updated-{Random.Shared.Next()}";
    await departmentClient.updateDepartmentAsync(departmentId, new DepartmentModificationModel(updatedName, null, createdEmployee.id), TestContext.Current.CancellationToken);

    // Assert
    var verificationModel = (await departmentClient.getDepartmentAsync(departmentId, TestContext.Current.CancellationToken)).Unwrap();

    verificationModel.ShouldNotBeNull();
    verificationModel.name.ShouldBe(updatedName);
  }
}
