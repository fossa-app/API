using System.Net;
using EasyDoubles;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web;
using Fossa.Bridge.Models.ApiModels;
using Fossa.Bridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanyControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;

  public CompanyControllerWithSystemLicense(
    CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateCompanyWithAdministratorAccessTokenAndUnlicensedCountryAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant102.ADMIN1");
    const string companyName = "Company-1412593541";

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.CreateCompanyAsync(new CompanyModificationModel(companyName, "KZ"), TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateCompanyWithAdministratorAccessTokenWithLicensedCountryAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    var licenseEasyStoreBucket = _factory.Services.GetRequiredService<IEasyStores>().ResolveBucket<long, object>("License");

    transport.SetAuthorizationToken("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string companyName = "Company-1993954667";

    await companyClient.CreateCompanyAsync(new CompanyModificationModel(companyName, "us"), TestContext.Current.CancellationToken);

    var responseModel = await companyClient.GetCompanyAsync(TestContext.Current.CancellationToken);

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.Name.ShouldBe(companyName);
    responseModel.CountryCode.ShouldBe("US");

    licenseEasyStoreBucket.BucketContent.Values.Where(x => string.Equals(x.Path, $"Company{responseModel.Id}", StringComparison.Ordinal)).ShouldNotBeEmpty();
  }

  [Fact]
  public async Task CreateCompanyWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.CreateCompanyAsync(new CompanyModificationModel("Company X", "US"), TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateCompanyWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");
    const string companyName = "Company-144764445";

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.CreateCompanyAsync(new CompanyModificationModel(companyName, "US"), TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteCompanyWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.DeleteCompanyAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingCompanyWithDependenciesWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.ADMIN1");

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.DeleteCompanyAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task DeleteExistingCompanyWithoutDependenciesWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();
    var companyEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<CompanyMongoEntity, long>();

    transport.SetAuthorizationToken("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant3.ADMIN1");

    companyEasyStore.Entities.Values.FirstOrDefault(x => string.Equals(x.Name, "Company3-1868946743", StringComparison.Ordinal)).ShouldNotBeNull();

    await companyClient.DeleteCompanyAsync(TestContext.Current.CancellationToken);

    companyEasyStore.Entities.Values.FirstOrDefault(x => string.Equals(x.Name, "Company3-1868946743", StringComparison.Ordinal)).ShouldBeNull();
  }

  [Fact]
  public async Task DeleteExistingCompanyWithUserAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.User1");

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.DeleteCompanyAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteMissingCompanyWithAdministratorAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1000.ADMIN1");

    await companyClient.DeleteCompanyAsync(TestContext.Current.CancellationToken);
    Assert.True(true);
  }

  public ValueTask DisposeAsync() => ValueTask.CompletedTask;

  public async ValueTask InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  [Fact]
  public async Task RetrieveCompanyWithoutAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.GetCompanyAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingCompanyWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.User1");
    var responseModel = await companyClient.GetCompanyAsync(TestContext.Current.CancellationToken);

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.Name.ShouldNotBeNullOrEmpty();
    responseModel.CountryCode.ShouldBe("US");
  }

  [Fact]
  public async Task RetrieveMissingCompanyWithAccessTokenAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var companyClient = scope.ServiceProvider.GetRequiredService<IClients>().CompanyClient;
    var transport = (TestHttpTransport)scope.ServiceProvider.GetRequiredService<IHttpTransport>();

    transport.SetAuthorizationToken("Bearer", "01J9SXMEMR1GQ3EE3Q4A872GKD.Tenant1000.User1000");

    var ex = await Should.ThrowAsync<HttpRequestException>(() => companyClient.GetCompanyAsync(TestContext.Current.CancellationToken));

    ex.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }
}
