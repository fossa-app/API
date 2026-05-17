using System.Net;
using Fossa.API.Web;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class SystemLicenseControllerEmptyDatabase : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public SystemLicenseControllerEmptyDatabase(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory;
  }

  [Fact]
  public async Task RetrieveSystemLicenseAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var systemLicenseClient = scope.ServiceProvider.GetRequiredService<IClients>().SystemLicenseClient;

    var ex = await Should.ThrowAsync<HttpRequestException>(() => systemLicenseClient.GetLicenseAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }
}
