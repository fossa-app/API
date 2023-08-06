using Fossa.API.Core;
using PublicApiGenerator;

namespace Fossa.API.UnitTests;

[UsesVerify]
public class PublicAPITests
{
  [Fact]
  public Task MyAssemblyHasNoPublicAPIChangesAsync()
  {
    var publicApi = typeof(DefaultCoreModule).Assembly.GeneratePublicApi();

    return Verify(publicApi);
  }
}
