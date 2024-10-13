using Microsoft.AspNetCore.Mvc.Testing;

namespace Fossa.API.FunctionalTests.Seed;

public static class AllEntitiesExtensions
{
  public static async Task SeedAllEntitiesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await factory.SeedCompaniesAsync(default).ConfigureAwait(false);
    await factory.SeedBranchesAsync(default).ConfigureAwait(false);
    await factory.SeedEmployeesAsync(default).ConfigureAwait(false);
  }
}

