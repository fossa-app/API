using Fossa.API.Web;
using Xunit;

namespace Fossa.API.FunctionalTests.ControllerApis;

[Collection("Sequential")]
public class MetaControllerInfo : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly HttpClient _client;

  public MetaControllerInfo(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsVersionAndLastUpdateDateAsync()
  {
    var response = await _client.GetAsync("/info");
    response.EnsureSuccessStatusCode();
    var stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("Version", stringResponse, StringComparison.Ordinal);
    Assert.Contains("Last Updated", stringResponse, StringComparison.Ordinal);
  }
}
