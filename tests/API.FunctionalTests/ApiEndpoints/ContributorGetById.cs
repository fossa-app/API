using Ardalis.HttpClientTestExtensions;
using Xunit;
using Fossa.API.Web.Endpoints.ContributorEndpoints;
using Fossa.API.Web;
using Ardalis.Result;
using System.Net;

namespace Fossa.API.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ContributorGetById : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public ContributorGetById(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsSeedContributorGivenId1()
  {
    var result = await _client.GetAndDeserializeAsync<ContributorRecord>(GetContributorByIdRequest.BuildRoute(1));

    Assert.Equal(1, result.Id);
    Assert.Equal(SeedData.Contributor1.Name, result.Name);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenId0()
  {
    var route = GetContributorByIdRequest.BuildRoute(0);
    var result = await _client.GetAndEnsureNotFoundAsync(route);

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }
}
