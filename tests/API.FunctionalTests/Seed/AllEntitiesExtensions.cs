using Microsoft.AspNetCore.Mvc.Testing;

namespace Fossa.API.FunctionalTests.Seed;

public static class AllEntitiesExtensions
{
  public static async Task SeedAllEntitiesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    await factory.SeedCompaniesAsync(cancellationToken).ConfigureAwait(false);
    await factory.SeedCompanyLicensesAsync(cancellationToken).ConfigureAwait(false);
    await factory.SeedCompanySettingsAsync(cancellationToken).ConfigureAwait(false);
    await factory.SeedBranchesAsync(cancellationToken).ConfigureAwait(false);
    await factory.SeedEmployeesAsync(cancellationToken).ConfigureAwait(false);
  }
}
