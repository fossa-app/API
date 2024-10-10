using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class BranchesControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public BranchesControllerWithSystemLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedCompaniesAsync(default).ConfigureAwait(false);
    await _factory.SeedBranchesAsync(default).ConfigureAwait(false);
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
}
