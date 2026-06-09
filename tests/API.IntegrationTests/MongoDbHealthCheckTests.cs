using Fossa.API.Web.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shouldly;

namespace Fossa.API.IntegrationTests;

public sealed class MongoDbHealthCheckTests(MongoDbContainerFixture mongoDb) : IClassFixture<MongoDbContainerFixture>
{
  [Fact]
  public async Task CheckHealthAsync_WithRunningMongoDbContainer_ShouldReportHealthy()
  {
    var healthCheck = new MongoDbHealthCheck(mongoDb.ConnectionString);
    var context = new HealthCheckContext
    {
      Registration = new HealthCheckRegistration(
        "mongodb",
        _ => healthCheck,
        HealthStatus.Unhealthy,
        tags: null),
    };

    var result = await healthCheck.CheckHealthAsync(context, TestContext.Current.CancellationToken);

    result.Status.ShouldBe(HealthStatus.Healthy);
    result.Exception.ShouldBeNull();
  }
}
