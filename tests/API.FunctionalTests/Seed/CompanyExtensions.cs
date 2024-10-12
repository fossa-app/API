using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class CompanyExtensions
{
  public static async Task SeedCompaniesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    var companyRepository = factory.Services.GetRequiredService<CompanyMongoEasyRepository>();

    await companyRepository.TryAddAsync(new CompanyMongoEntity
    {
      ID = 100L,
      TenantID = Guid.Parse("53ade3c2-8e36-52f2-88cf-d068b1ab247a"),
      Name = "Company1",
    }, cancellationToken).ConfigureAwait(false);

    await companyRepository.TryAddAsync(new CompanyMongoEntity
    {
      ID = 200L,
      TenantID = Guid.Parse("cf59d3dd-5258-5a20-88ab-0169cf128440"),
      Name = "Company2",
    }, cancellationToken).ConfigureAwait(false);
  }
}
