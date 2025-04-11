using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Extensions;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class EmployeesControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

    public EmployeesControllerWithSystemLicense(
        CustomWebApplicationFactory<DefaultWebModule> factory,
        ITestOutputHelper testOutputHelper)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Theory]
    [InlineData(0, 10)]   // Invalid page number
    [InlineData(1, 0)]    // Invalid page size
    [InlineData(-1, 10)]  // Negative page number
    [InlineData(1, -1)]   // Negative page size
    [InlineData(1, 1001)] // Page size too large
    public async Task ListEmployees_WithInvalidPaging_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

        var response = await client.GetAsync($"/api/1.0/Employees?pageNumber={pageNumber}&pageSize={pageSize}");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 3)]
    [InlineData(1, 10)]
    public async Task ListEmployees_WithValidPaging_ReturnsPaginatedResults(int pageNumber, int pageSize)
    {
        // Create multiple employees first
        var client = _factory.CreateClient();
        var userTokens = new[]
        {
            "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1",
            "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User2",
            "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User3",
            "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User4",
            "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User5"
        };

        foreach (var token in userTokens)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("/api/1.0/Employee",
                new EmployeeModificationModel($"First{token}", $"Last{token}", $"Full{token}"));
            response.EnsureSuccessStatusCode();
        }

        // Get paginated results
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");
        var listResponse = await client.GetAsync($"/api/1.0/Employees?pageNumber={pageNumber}&pageSize={pageSize}");

        listResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await listResponse.Content.ReadFromJsonAsync<PagingResponseModel<EmployeeRetrievalModel>>();

        result.ShouldNotBeNull();
        result.PageNumber.ShouldBe(pageNumber);
        result.PageSize.ShouldBe(pageSize);
        result.Items.Count.ShouldBeLessThanOrEqualTo(pageSize);
    }

    [Fact]
    public async Task ListEmployees_WithSearchTerm_ReturnsFilteredResults()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");

        // Create employees with distinct names using different user tokens
        var employees = new[]
        {
            (Token: "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1", First: "John", Last: "Smith", Full: "John Smith"),
            (Token: "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User2", First: "Jane", Last: "Smith", Full: "Jane Smith"),
            (Token: "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User3", First: "Bob", Last: "Jones", Full: "Bob Jones")
        };

        foreach (var emp in employees)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", emp.Token);
            await client.PostAsJsonAsync("/api/1.0/Employee",
                new EmployeeModificationModel(emp.First, emp.Last, emp.Full));
        }

        // Search for Smith employees
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");
        var response = await client.GetAsync("/api/1.0/Employees?search=Smith&pageNumber=1&pageSize=10");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagingResponseModel<EmployeeRetrievalModel>>();

        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.Items.All(x => x.LastName == "Smith").ShouldBeTrue();
    }

    [Fact]
    public async Task ListEmployees_WithSpecificIds_ReturnsRequestedEmployees()
    {
        var client = _factory.CreateClient();
        var employeeIds = new List<long>();

        // Create employees with different user tokens
        var employees = new[]
        {
            (Token: "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User1", First: "John", Last: "Doe", Full: "John Doe"),
            (Token: "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User2", First: "Jane", Last: "Smith", Full: "Jane Smith"),
            (Token: "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.User3", First: "Bob", Last: "Jones", Full: "Bob Jones")
        };

        foreach (var emp in employees)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", emp.Token);
            var response1 = await client.PostAsJsonAsync("/api/1.0/Employee",
                new EmployeeModificationModel(emp.First, emp.Last, emp.Full));
            response1.EnsureSuccessStatusCode();

            var empResponse = await client.GetAsync("/api/1.0/Employee");
            var empModel = await empResponse.Content.ReadFromJsonAsync<EmployeeRetrievalModel>();
            empModel.ShouldNotBeNull();
            employeeIds.Add(empModel.Id);
        }

        // Request specific employees by ID using admin token
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant1.ADMIN1");
        var response2 = await client.GetAsync($"/api/1.0/Employees?id={employeeIds[0]}&id={employeeIds[1]}");

        response2.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response2.Content.ReadFromJsonAsync<PagingResponseModel<EmployeeRetrievalModel>>();

        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.Items.Select(x => x.Id).ShouldContain(employeeIds[0]);
        result.Items.Select(x => x.Id).ShouldContain(employeeIds[1]);
        result.Items.Select(x => x.Id).ShouldNotContain(employeeIds[2]);
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
        await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
