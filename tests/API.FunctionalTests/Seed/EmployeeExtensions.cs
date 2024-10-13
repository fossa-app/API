using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Seed;

public static class EmployeeExtensions
{
  public static async Task SeedEmployeesAsync<TEntryPoint>(
    this WebApplicationFactory<TEntryPoint> factory,
    CancellationToken cancellationToken)
    where TEntryPoint : class
  {
    var EmployeeRepository = factory.Services.GetRequiredService<EmployeeMongoEasyRepository>();

    await EmployeeRepository.TryAddAsync(new EmployeeMongoEntity
    {
      ID = 10000L,
      CompanyId = 100L,
      TenantID = Guid.Parse("53ade3c2-8e36-52f2-88cf-d068b1ab247a"),
      UserID = Guid.Parse("b7d80904-d65c-5469-912c-899e403d91db"),
      FirstName = "Employee1F",
      LastName = "Employee1L",
      FullName = "Employee1FL",
    }, cancellationToken).ConfigureAwait(false);

    await EmployeeRepository.TryAddAsync(new EmployeeMongoEntity
    {
      ID = 20000L,
      CompanyId = 100L,
      TenantID = Guid.Parse("53ade3c2-8e36-52f2-88cf-d068b1ab247a"),
      UserID = Guid.Parse("ba463b27-82c1-59cc-b5d6-596622ac6998"),
      FirstName = "Employee2F",
      LastName = "Employee2L",
      FullName = "Employee2FL",
    }, cancellationToken).ConfigureAwait(false);
  }
}
