using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyDoubles;
using Fossa.API.Core.Services;
using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.FunctionalTests.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fossa.API.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
  where TProgram : class
{
  public Task InitializeAsync()
  {
    var systemInitializer = Services.GetRequiredService<ISystemInitializer>();

    return systemInitializer.InitializeAsync(default);
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await base.DisposeAsync().ConfigureAwait(false);
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder
      .ConfigureServices(services => services.AddEasyDoubles());
  }

  /// <summary>
  ///   Overriding CreateHost to avoid creating a separate ServiceProvider per this thread:
  ///   https://github.com/dotnet-architecture/eShopOnWeb/issues/465
  /// </summary>
  /// <param name="builder"></param>
  /// <returns></returns>
  protected override IHost CreateHost(IHostBuilder builder)
  {
    builder.UseEnvironment("Development");
    builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
      containerBuilder
        .RegisterType<LicenseEasyFileRepository>()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<TestCertificateProvider>()
        .AsImplementedInterfaces()
        .SingleInstance();
    });
    var host = builder.Build();
    host.Start();

    // Get service provider.
    var serviceProvider = host.Services;

    // Create a scope to obtain a reference to the database
    // context (AppDbContext).
    using (var scope = serviceProvider.CreateScope())
    {
      // scope.ServiceProvider.GetRequiredService<>()
    }

    return host;
  }
}
