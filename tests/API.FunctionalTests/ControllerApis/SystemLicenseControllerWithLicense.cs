using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Web;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class SystemLicenseControllerWithLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public SystemLicenseControllerWithLicense(CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task RetrieveSystemLicenseAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var systemLicenseClient = scope.ServiceProvider.GetRequiredService<IClients>().SystemLicenseClient;

    var licenseResponseModel = (await systemLicenseClient.GetLicenseAsync(TestContext.Current.CancellationToken)).Unwrap();

    licenseResponseModel.ShouldNotBeNull();
    licenseResponseModel.Terms.ShouldNotBeNull();
    licenseResponseModel.Terms.Licensor.ShouldNotBeNull();
    licenseResponseModel.Terms.Licensee.ShouldNotBeNull();
    licenseResponseModel.Entitlements.ShouldNotBeNull();
    licenseResponseModel.Entitlements.EnvironmentName.ShouldNotBeNull();
    licenseResponseModel.Entitlements.EnvironmentKind.ShouldNotBeNull();
    licenseResponseModel.Entitlements.Countries.ShouldNotBeEmpty();
    licenseResponseModel.Entitlements.TimeZones.ShouldNotBeEmpty();
    licenseResponseModel.Entitlements.MaximumCompanyCount.ShouldBePositive();
  }

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
