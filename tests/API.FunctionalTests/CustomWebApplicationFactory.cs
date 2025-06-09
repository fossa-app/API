using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyDoubles;
using Fossa.API.Core.Services;
using Fossa.API.FunctionalTests.Repositories;
using Fossa.API.FunctionalTests.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Fossa.API.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
  where TProgram : class
{
  async Task IAsyncLifetime.DisposeAsync()
  {
    await base.DisposeAsync().ConfigureAwait(false);
  }

  public Task InitializeAsync()
  {
    var systemInitializer = Services.GetRequiredService<ISystemInitializer>();

    return systemInitializer.InitializeAsync(default);
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder
      .ConfigureServices(services =>
      {
        services.AddEasyDoubles();
        services.AddAuthentication("Test")
          .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
      })
      .ConfigureAppConfiguration(configure =>
      {
        var secretsSources = configure.Sources.OfType<JsonConfigurationSource>().Where(x => ((PhysicalFileProvider)x.FileProvider!).Root.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StringComparison.OrdinalIgnoreCase)).ToList();
        secretsSources.ForEach(x => configure.Sources.Remove(x));
      });
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
        .RegisterType<SystemPropertiesMongoEasyRepository>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<CompanyMongoEasyRepository>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<CompanySettingsMongoEasyRepository>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<BranchMongoEasyRepository>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<EmployeeMongoEasyRepository>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<DepartmentMongoEasyRepository>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      containerBuilder
        .RegisterType<LicenseEasyFileRepository>()
        .AsSelf()
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
