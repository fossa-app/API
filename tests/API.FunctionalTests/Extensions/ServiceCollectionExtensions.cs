using Fossa.Bridge.Services;
using Fossa.Bridge.Services.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.FunctionalTests.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddBridge(this IServiceCollection services)
  {
    _ = services.AddScoped<TestHttpTransportWithAccessToken>();
    _ = services.AddScoped<IHttpTransport>(sp => sp.GetRequiredService<TestHttpTransportWithAccessToken>());
    _ = services.AddScoped<IAccessTokenProvider>(sp => sp.GetRequiredService<TestHttpTransportWithAccessToken>());
    _ = services.AddScoped<IAccessTokenContext>(sp => sp.GetRequiredService<TestHttpTransportWithAccessToken>());
    _ = services.AddScoped<IClients, Clients>();

    return services;
  }
}
