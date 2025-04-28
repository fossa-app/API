using System.DirectoryServices.Protocols;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class DepartmentsControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public DepartmentsControllerWithSystemLicense(
      CustomWebApplicationFactory<DefaultWebModule> factory,
      ITestOutputHelper testOutputHelper)
  {
    _factory = factory;
    _testOutputHelper = testOutputHelper;
  }

  [Fact]
  public async Task CreateAndListDepartmentsAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN420425736");

    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    var dept1Name = $"Department-{Random.Shared.Next()}";
    var dept2Name = $"Department-{Random.Shared.Next()}";
    var dept3Name = $"Department-{Random.Shared.Next()}";

    var employeeCreationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));
    employeeCreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var employeeRetrievalResponse = await client.GetAsync("/api/1.0/Employee");
    var employeeResponseModel =
      await employeeRetrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeResponseModel.ShouldNotBeNull();

    // Act - Create departments
    var dept1CreationResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
        new DepartmentModificationModel(dept1Name, null, employeeResponseModel.Id));
    var dept2CreationResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
        new DepartmentModificationModel(dept2Name, null, employeeResponseModel.Id));
    var dept3CreationResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
        new DepartmentModificationModel(dept3Name, null, employeeResponseModel.Id));

    // Assert creation responses
    if (dept1CreationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await dept1CreationResponse.Content.ReadAsStringAsync());
    }
    dept1CreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    if (dept2CreationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await dept2CreationResponse.Content.ReadAsStringAsync());
    }
    dept2CreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    if (dept3CreationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await dept3CreationResponse.Content.ReadAsStringAsync());
    }
    dept3CreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Get the created departments
    var retrievalResponse = await client.GetAsync("/api/1.0/Departments?pageNumber=1&pageSize=100");
    var retrievalResponseModel = await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();

    retrievalResponseModel.ShouldNotBeNull();
    var dept1Model = retrievalResponseModel.Items.Single(x => string.Equals(x.Name, dept1Name, StringComparison.OrdinalIgnoreCase));
    var dept2Model = retrievalResponseModel.Items.Single(x => string.Equals(x.Name, dept2Name, StringComparison.OrdinalIgnoreCase));
    var dept3Model = retrievalResponseModel.Items.Single(x => string.Equals(x.Name, dept3Name, StringComparison.OrdinalIgnoreCase));

    var dept1Id = dept1Model.Id;
    var dept2Id = dept2Model.Id;
    var dept3Id = dept3Model.Id;
    const int dept4Id = 204298046; // Missing department

    // Query specific departments
    var deptRetrievalResponse = await client.GetAsync($"/api/1.0/Departments?Id={dept1Id}&Id={dept2Id}&Id={dept3Id}&Id={dept4Id}");
    deptRetrievalResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var deptResponseModel = await deptRetrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();

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

    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN430851539");

    var employeeCreationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));
    employeeCreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var employeeRetrievalResponse = await client.GetAsync("/api/1.0/Employee");
    var employeeResponseModel =
      await employeeRetrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeResponseModel.ShouldNotBeNull();

    var departmentName = $"Department-{Random.Shared.Next()}";
    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
        new DepartmentModificationModel(departmentName, null, employeeResponseModel.Id));
    if (creationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await creationResponse.Content.ReadAsStringAsync());
    }
    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Departments?pageNumber=1&pageSize=100");
    var retrievalResponseModel = await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();
    var department = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, departmentName, StringComparison.OrdinalIgnoreCase));

    // Act
    var deleteResponse = await client.DeleteAsync($"/api/1.0/Departments/{department?.Id}");

    // Assert
    if (deleteResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await deleteResponse.Content.ReadAsStringAsync());
    }
    deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var verificationResponse = await client.GetAsync($"/api/1.0/Departments/{department?.Id}");
    verificationResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task UpdateExistingDepartmentWithAdministratorAccessTokenAsync()
  {
    // Arrange
    const string firstName = "First35292075";
    const string lastName = "Last35292075";
    const string fullName = "Full35292075";

    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JMV0XC70JH9GC8P9M6SYYYAK.Tenant1.ADMIN604735919");

    var employeeCreationResponse = await client.PostAsJsonAsync("/api/1.0/Employee", new EmployeeModificationModel(firstName, lastName, fullName));
    employeeCreationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var employeeRetrievalResponse = await client.GetAsync("/api/1.0/Employee");
    var employeeResponseModel =
      await employeeRetrievalResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
    employeeResponseModel.ShouldNotBeNull();

    var departmentName = $"Department-{Random.Shared.Next()}";
    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Departments",
        new DepartmentModificationModel(departmentName, null, employeeResponseModel.Id));
    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Departments?pageNumber=1&pageSize=100");
    var retrievalResponseModel = await retrievalResponse.Content.ReadFromJsonAsync<PagingResponseModel<DepartmentRetrievalModel>>();
    var departmentId = retrievalResponseModel?.Items.Single(x => string.Equals(x.Name, departmentName, StringComparison.OrdinalIgnoreCase)).Id;

    // Act
    var updatedName = $"Updated-{Random.Shared.Next()}";
    var modificationResponse = await client.PutAsJsonAsync($"/api/1.0/Departments/{departmentId}",
        new DepartmentModificationModel(updatedName, null, employeeResponseModel.Id));

    // Assert
    if (modificationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await modificationResponse.Content.ReadAsStringAsync());
    }
    modificationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var verificationResponse = await client.GetAsync($"/api/1.0/Departments/{departmentId}");
    verificationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var verificationModel = await verificationResponse.Content.ReadFromJsonAsync<DepartmentRetrievalModel>();
    verificationModel.ShouldNotBeNull();
    verificationModel.Name.ShouldBe(updatedName);
  }
}
