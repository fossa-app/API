using Fossa.Bridge.Services;
using Fossa.Bridge.Services.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddBridge(this IServiceCollection services)
  {
    _ = services.AddScoped<IHttpTransport, TestHttpTransport>();
    _ = services.AddScoped<IClients, Clients>();

    return services;
  }
}
