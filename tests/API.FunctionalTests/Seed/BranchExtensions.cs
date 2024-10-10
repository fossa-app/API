using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class BranchExtensions
{
  public static async Task SeedBranchesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    var branchRepository = factory.Services.GetRequiredService<BranchMongoEasyRepository>();

    await branchRepository.TryAddAsync(new BranchMongoEntity
    {
      ID = 1000L,
      CompanyId = 100L,
      TenantID = Guid.Parse("53ade3c2-8e36-52f2-88cf-d068b1ab247a"),
      Name = "Branch1",
    }, cancellationToken).ConfigureAwait(false);
  }
}
