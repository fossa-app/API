using Fossa.API.Core;
using Fossa.API.Infrastructure;
using Fossa.API.Persistence;
using Fossa.API.Web;
using PublicApiGenerator;

namespace Fossa.API.UnitTests;

public class PublicAPITests
{
  private readonly ApiGeneratorOptions _apiGeneratorOptions = new() { IncludeAssemblyAttributes = false };

  [Fact]
  public Task CoreAssemblyHasNoPublicAPIChangesAsync()
  {
    var publicApi = typeof(DefaultCoreModule).Assembly.GeneratePublicApi(_apiGeneratorOptions);

    return Verify(publicApi);
  }

  [Fact]
  public Task InfrastructureAssemblyHasNoPublicAPIChangesAsync()
  {
    var publicApi = typeof(DefaultInfrastructureModule).Assembly.GeneratePublicApi(_apiGeneratorOptions);

    return Verify(publicApi);
  }

  [Fact]
  public Task PersistenceAssemblyHasNoPublicAPIChangesAsync()
  {
    var publicApi = typeof(DefaultPersistenceModule).Assembly.GeneratePublicApi(_apiGeneratorOptions);

    return Verify(publicApi);
  }

  [Fact]
  public Task WebAssemblyHasNoPublicAPIChangesAsync()
  {
    var publicApi = typeof(DefaultWebModule).Assembly.GeneratePublicApi(_apiGeneratorOptions);

    return Verify(publicApi);
  }
}
