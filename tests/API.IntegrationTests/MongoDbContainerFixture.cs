using Testcontainers.MongoDb;

namespace Fossa.API.IntegrationTests;

public sealed class MongoDbContainerFixture : IAsyncLifetime
{
  private readonly MongoDbContainer _container = new MongoDbBuilder("mongo:8.0")
    .Build();

  public string ConnectionString => _container.GetConnectionString();

  public async ValueTask InitializeAsync()
  {
    await _container.StartAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
  }

  public async ValueTask DisposeAsync()
  {
    await _container.DisposeAsync().ConfigureAwait(false);
  }
}
