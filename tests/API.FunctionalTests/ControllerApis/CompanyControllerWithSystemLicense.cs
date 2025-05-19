﻿using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EasyDoubles;
using Fossa.API.FunctionalTests.Seed;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Web;
using Fossa.API.Web.ApiModels;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace Fossa.API.FunctionalTests.ControllerApis;

public class CompanyControllerWithSystemLicense : IClassFixture<CustomWebApplicationFactory<DefaultWebModule>>, IAsyncLifetime
{
  private readonly CustomWebApplicationFactory<DefaultWebModule> _factory;
  private readonly ITestOutputHelper _testOutputHelper;

  public CompanyControllerWithSystemLicense(
    ITestOutputHelper testOutputHelper,
    CustomWebApplicationFactory<DefaultWebModule> factory)
  {
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    _factory = factory ?? throw new ArgumentNullException(nameof(factory));
  }

  [Fact]
  public async Task CreateCompanyWithAdministratorAccessTokenAndUnlicensedCountryAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant102.ADMIN1");
    const string companyName = "Company-1412593541";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "KZ"));

    if (creationResponse.StatusCode != HttpStatusCode.OK)
    {
      _testOutputHelper.WriteLine(await creationResponse.Content.ReadAsStringAsync());
    }

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateCompanyWithAdministratorAccessTokenWithLicensedCountryAsync()
  {
    var client = _factory.CreateClient();
    var licenseEasyStoreBucket = _factory.Services.GetRequiredService<IEasyStores>().ResolveBucket<long, object>("License");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string companyName = "Company-1993954667";

    var creationResponse = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "us"));

    creationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Company");

    var responseModel =
      await retrievalResponse.Content.ReadFromJsonAsync<CompanyRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.Name.ShouldBe(companyName);
    responseModel.CountryCode.ShouldBe("US");

    licenseEasyStoreBucket.BucketContent.Values.Where(x => string.Equals(x.Path, $"Company{responseModel.Id}", StringComparison.Ordinal)).ShouldNotBeEmpty();
  }

  [Fact]
  public async Task CreateCompanyWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel("Company X", "US"));

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task CreateCompanyWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");
    const string companyName = "Company-144764445";
    var response = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "US"));

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteCompanyWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task DeleteExistingCompanyWithDependenciesWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.ADMIN1");
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
  }

  [Fact]
  public async Task DeleteExistingCompanyWithoutDependenciesWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var companyEasyStore = _factory.Services.GetRequiredService<IEasyStores>().Resolve<CompanyMongoEntity, long>();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant3.ADMIN1");

    companyEasyStore.Entities.Values.FirstOrDefault(x => string.Equals(x.Name, "Company3-1868946743", StringComparison.Ordinal)).ShouldNotBeNull();

    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    companyEasyStore.Entities.Values.FirstOrDefault(x => string.Equals(x.Name, "Company3-1868946743", StringComparison.Ordinal)).ShouldBeNull();
  }

  [Fact]
  public async Task DeleteExistingCompanyWithUserAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.User1");
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task DeleteMissingCompanyWithAdministratorAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1000.ADMIN1");
    var response = await client.DeleteAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.OK);
  }

  public Task DisposeAsync() => Task.CompletedTask;

  public async Task InitializeAsync()
  {
    await _factory.SeedSystemLicenseAsync(default).ConfigureAwait(false);
    await _factory.SeedAllEntitiesAsync(default).ConfigureAwait(false);
  }

  [Fact]
  public async Task RetrieveCompanyWithoutAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task RetrieveExistingCompanyWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SJ94KK62JSRNQD7H70NCF7.Tenant1.User1");
    var response = await client.GetAsync("/api/1.0/Company");
    response.EnsureSuccessStatusCode();

    var responseModel =
      await response.Content.ReadFromJsonAsync<CompanyRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Id.ShouldBePositive();
    responseModel.Name.ShouldNotBeNullOrEmpty();
    responseModel.CountryCode.ShouldBe("US");
  }

  [Fact]
  public async Task RetrieveMissingCompanyWithAccessTokenAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01J9SXMEMR1GQ3EE3Q4A872GKD.Tenant1000.User1000");
    var response = await client.GetAsync("/api/1.0/Company");

    response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UpdateCompanyWithAdministratorAccessTokenWithValidDataAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string modifiedName = "Modified Company Name";
    const string modifiedCountryCode = "US";

    // Act
    var updateResponse = await client.PutAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(modifiedName, modifiedCountryCode));

    // Assert
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Company");
    var responseModel = await retrievalResponse.Content.ReadFromJsonAsync<CompanyRetrievalModel>();

    responseModel.ShouldNotBeNull();
    responseModel.Name.ShouldBe(modifiedName);
    responseModel.CountryCode.ShouldBe(modifiedCountryCode);
  }

  [Fact]
  public async Task UpdateCompanyWithAdministratorAccessTokenWithInvalidCountryAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string modifiedName = "Modified Company Name";
    const string invalidCountryCode = "XX"; // Invalid country code

    // Act
    var updateResponse = await client.PutAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(modifiedName, invalidCountryCode));

    // Assert
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateCompanyWithUserAccessTokenAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJFK2J690FS0Q3TCX4P3F.Tenant101.User1");
    const string modifiedName = "Modified Company Name";
    const string modifiedCountryCode = "US";

    // Act
    var response = await client.PutAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(modifiedName, modifiedCountryCode));

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
  }

  [Fact]
  public async Task UpdateCompanyWithoutAccessTokenAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    const string modifiedName = "Modified Company Name";
    const string modifiedCountryCode = "US";

    // Act
    var response = await client.PutAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(modifiedName, modifiedCountryCode));

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
  }

  [Theory]
  [InlineData(null, "US")]
  [InlineData("", "US")]
  [InlineData("Test Company", null)]
  [InlineData("Test Company", "")]
  public async Task UpdateCompanyWithAdministratorAccessTokenWithEmptyValuesAsync(string? name, string? countryCode)
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    // Act
    var updateResponse = await client.PutAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(name, countryCode));

    // Assert
    updateResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateCompanyWithAdministratorAccessTokenAndUnlicensedCountryAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant102.ADMIN1");

    var response = await client.PutAsync("/api/1.0/Company",
        JsonContent.Create(new CompanyModificationModel("UpdatedCompany", "KZ")));

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task UpdateCompanyWithAdministratorAccessTokenWithLicensedCountryAsync()
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string updatedCompanyName = "Company-Updated-40264474";

    var response = await client.PutAsync("/api/1.0/Company",
        JsonContent.Create(new CompanyModificationModel(updatedCompanyName, "US")));

    response.StatusCode.ShouldBe(HttpStatusCode.OK);

    var retrievalResponse = await client.GetAsync("/api/1.0/Company");

    var responseModel = await retrievalResponse.Content.ReadFromJsonAsync<CompanyRetrievalModel>();
    responseModel.ShouldNotBeNull();
    responseModel.Name.ShouldBe(updatedCompanyName);
    responseModel.CountryCode.ShouldBe("US");
  }

  [Theory]
  [InlineData("a")]  // Too short
  [InlineData("ThisCompanyNameIsTooLongAndShouldNotBeAllowedAsItExceedsTheMaximumLengthLimitForCompanyNamesInTheSystem")]  // Too long
  [InlineData("Company@123")]  // Invalid characters
  [InlineData(" CompanyName ")] // Leading/trailing spaces
  public async Task CreateCompanyWithInvalidNameFormatAsync(string invalidCompanyName)
  {
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    var response = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(invalidCompanyName, "US"));

    response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
  }

  [Fact]
  public async Task CreateDuplicateCompanyNameAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string companyName = "DuplicateCompanyTest";

    // Create first company
    var firstResponse = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "US"));
    firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

    // Attempt to create company with same name
    var duplicateResponse = await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "US"));

    duplicateResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
  }

  [Fact]
  public async Task UpdateCompanyToExistingNameAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    // Create first company
    await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel("Company-A", "US"));

    // Create second company
    await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel("Company-B", "US"));

    // Attempt to update second company to first company's name
    var updateResponse = await client.PutAsJsonAsync("/api/1.0/Company", new CompanyModificationModel("Company-A", "US"));

    updateResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
  }

  [Fact]
  public async Task ConcurrentCompanyCreationAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");
    const string companyName = "ConcurrentCompany";

    // Act
    // Create multiple tasks that attempt to create a company with the same name simultaneously
    var tasks = Enumerable.Range(1, 3)
        .Select(_ => client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel(companyName, "US")))
        .ToList();

    var results = await Task.WhenAll(tasks);

    // Assert
    // Only one request should succeed, others should fail with conflict
    results.Count(r => r.StatusCode == HttpStatusCode.OK).ShouldBe(1);
    results.Count(r => r.StatusCode == HttpStatusCode.Conflict).ShouldBe(2);
  }

  [Fact]
  public async Task ConcurrentCompanyUpdateAsync()
  {
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "01JA1ZJAWF27S0J8Z2VJE7673Y.Tenant101.ADMIN1");

    // Create initial company
    await client.PostAsJsonAsync("/api/1.0/Company", new CompanyModificationModel("ConcurrentUpdateTest", "US"));

    // Act
    // Create multiple tasks that attempt to update the company simultaneously
    var tasks = Enumerable.Range(1, 3)
        .Select(i => client.PutAsJsonAsync("/api/1.0/Company",
            new CompanyModificationModel($"Updated Company {i}", "US")))
        .ToList();

    var results = await Task.WhenAll(tasks);

    // Assert
    // Only one update should succeed
    results.Count(r => r.StatusCode == HttpStatusCode.OK).ShouldBe(1);

    // Verify final state
    var getResponse = await client.GetAsync("/api/1.0/Company");
    var company = await getResponse.Content.ReadFromJsonAsync<CompanyRetrievalModel>();
    company.ShouldNotBeNull();
    company.Name.StartsWith("Updated Company").ShouldBeTrue();
  }
}
