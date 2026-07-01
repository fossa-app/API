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

    var licenseResponseModel = (await systemLicenseClient.getLicenseAsync(TestContext.Current.CancellationToken)).Unwrap();

    licenseResponseModel.ShouldNotBeNull();
    licenseResponseModel.terms.ShouldNotBeNull();
    licenseResponseModel.terms.licensor.ShouldNotBeNull();
    licenseResponseModel.terms.licensee.ShouldNotBeNull();
    licenseResponseModel.entitlements.ShouldNotBeNull();
    licenseResponseModel.entitlements.environmentName.ShouldNotBeNull();
    licenseResponseModel.entitlements.environmentKind.ShouldNotBeNull();
    licenseResponseModel.entitlements.countries.ShouldNotBeEmpty();
    licenseResponseModel.entitlements.timeZones.ShouldNotBeEmpty();
    licenseResponseModel.entitlements.maximumCompanyCount.ShouldBePositive();
  }

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
