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
      UserID = Guid.Parse("b8e8f25a-0d5c-58bd-8009-a6b50d0d4d69"),
      FirstName = "Employee1F",
      LastName = "Employee1L",
      FullName = "Employee1FL",
    }, cancellationToken).ConfigureAwait(false);

    await EmployeeRepository.TryAddAsync(new EmployeeMongoEntity
    {
      ID = 20000L,
      CompanyId = 100L,
      TenantID = Guid.Parse("53ade3c2-8e36-52f2-88cf-d068b1ab247a"),
      UserID = Guid.Parse("af619381-46ce-59e8-8e5e-7940b8119a88"),
      FirstName = "Employee2F",
      LastName = "Employee2L",
      FullName = "Employee2FL",
    }, cancellationToken).ConfigureAwait(false);
  }
}
