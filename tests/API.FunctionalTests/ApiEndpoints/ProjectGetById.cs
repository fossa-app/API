using Ardalis.HttpClientTestExtensions;
using Xunit;
using Fossa.API.Web.Endpoints.ProjectEndpoints;
using Fossa.API.Web;
using System.Net;

namespace Fossa.API.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ProjectGetById : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public ProjectGetById(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsSeedProjectGivenId1Async()
  {
    var result = await _client.GetAndDeserializeAsync<GetProjectByIdResponse>(GetProjectByIdRequest.BuildRoute(1));

    Assert.Equal(1, result.Id);
    Assert.Equal(SeedData.TestProject1.Name, result.Name);
    Assert.Equal(3, result.Items.Count);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenId0Async()
  {
    var route = GetProjectByIdRequest.BuildRoute(0);
    var result = await _client.GetAndEnsureNotFoundAsync(route);

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }
}
