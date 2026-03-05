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
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN420425736");

    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    var dept1Name = $"Department-{Random.Shared.Next()}";
    var dept2Name = $"Department-{Random.Shared.Next()}";
    var dept3Name = $"Department-{Random.Shared.Next()}";

    await employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var createdEmployee = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);
    createdEmployee.ShouldNotBeNull();

    // Act - Create departments
    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel(dept1Name, null, createdEmployee.Id), TestContext.Current.CancellationToken);
    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel(dept2Name, null, createdEmployee.Id), TestContext.Current.CancellationToken);
    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel(dept3Name, null, createdEmployee.Id), TestContext.Current.CancellationToken);

    // Get the created departments
    var retrievalResponseModel = await departmentClient.GetDepartmentsAsync(new DepartmentQueryRequestModel { PageNumber = 1, PageSize = 100 }, TestContext.Current.CancellationToken);

    retrievalResponseModel.ShouldNotBeNull();
    var dept1Model = retrievalResponseModel.Items.Single(x => string.Equals(x.Name, dept1Name, StringComparison.OrdinalIgnoreCase));
    var dept2Model = retrievalResponseModel.Items.Single(x => string.Equals(x.Name, dept2Name, StringComparison.OrdinalIgnoreCase));
    var dept3Model = retrievalResponseModel.Items.Single(x => string.Equals(x.Name, dept3Name, StringComparison.OrdinalIgnoreCase));

    var dept1Id = dept1Model.Id;
    var dept2Id = dept2Model.Id;
    var dept3Id = dept3Model.Id;
    const int dept4Id = 204298046; // Missing department

    // Query specific departments
    var deptResponseModel = await departmentClient.GetDepartmentsAsync(new DepartmentQueryRequestModel { Id = [dept1Id, dept2Id, dept3Id, dept4Id] }, TestContext.Current.CancellationToken);

    deptResponseModel.ShouldNotBeNull();
    deptResponseModel.Items.Count.ShouldBe(3);
    deptResponseModel.Items.ShouldContain(x => x.Id == dept1Id);
    deptResponseModel.Items.ShouldContain(x => x.Id == dept2Id);
    deptResponseModel.Items.ShouldContain(x => x.Id == dept3Id);
    deptResponseModel.Items.ShouldNotContain(x => x.Id == dept4Id);
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
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN430851539");

    await employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var createdEmployee = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);
    createdEmployee.ShouldNotBeNull();

    var departmentName = $"Department-{Random.Shared.Next()}";

    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel(departmentName, null, createdEmployee.Id), TestContext.Current.CancellationToken);

    var retrievalResponseModel = await departmentClient.GetDepartmentsAsync(new DepartmentQueryRequestModel { PageNumber = 1, PageSize = 100 }, TestContext.Current.CancellationToken);
    var department = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, departmentName, StringComparison.OrdinalIgnoreCase));

    // Act
    await departmentClient.DeleteDepartmentAsync(department!.Id, TestContext.Current.CancellationToken);

    // Assert
    var ex = await Should.ThrowAsync<HttpRequestException>(() => departmentClient.GetDepartmentAsync(department.Id, TestContext.Current.CancellationToken));
    ex.StatusCode.ShouldBe(HttpStatusCode.NotFound);
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
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN604735919");

    await employeeClient.CreateEmployeeAsync(new EmployeeModificationModel(firstName, lastName, fullName), TestContext.Current.CancellationToken);

    var createdEmployee = await employeeClient.GetCurrentEmployeeAsync(TestContext.Current.CancellationToken);
    createdEmployee.ShouldNotBeNull();

    var departmentName = $"Department-{Random.Shared.Next()}";
    await departmentClient.CreateDepartmentAsync(new DepartmentModificationModel(departmentName, null, createdEmployee.Id), TestContext.Current.CancellationToken);

    var retrievalResponseModel = await departmentClient.GetDepartmentsAsync(new DepartmentQueryRequestModel { PageNumber = 1, PageSize = 100 }, TestContext.Current.CancellationToken);
    var departmentId = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, departmentName, StringComparison.OrdinalIgnoreCase)).Id;

    // Act
    var updatedName = $"Updated-{Random.Shared.Next()}";
    await departmentClient.UpdateDepartmentAsync(departmentId!.Value, new DepartmentModificationModel(updatedName, null, createdEmployee.Id), TestContext.Current.CancellationToken);

    // Assert
    var verificationModel = await departmentClient.GetDepartmentAsync(departmentId.Value, TestContext.Current.CancellationToken);

    verificationModel.ShouldNotBeNull();
    verificationModel.Name.ShouldBe(updatedName);
  }
}
