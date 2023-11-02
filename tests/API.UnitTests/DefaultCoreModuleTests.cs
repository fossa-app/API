using Autofac;
using Autofac.Extensions.DependencyInjection;
using Fossa.API.Core;
using Fossa.API.Core.Entities;
using IdGen.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TIKSN.DependencyInjection;
using TIKSN.Identity;
using TIKSN.Mapping;
using Xunit.Abstractions;

namespace Fossa.API.UnitTests;

public class DefaultCoreModuleTests
{
  private readonly ITestOutputHelper _testOutputHelper;
  private readonly IServiceProvider _serviceProvider;

  public DefaultCoreModuleTests(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));

    var host = Host.CreateDefaultBuilder()
      .UseServiceProviderFactory(new AutofacServiceProviderFactory())
      .ConfigureServices(services =>
      {
        services.AddFrameworkCore();
        services.AddIdGen(0);

        services.Scan(scan => scan
          .FromApplicationDependencies()
          .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
          .AsImplementedInterfaces());
      })
      .ConfigureContainer<ContainerBuilder>(containerBuilder =>
      {
        containerBuilder.RegisterModule<CoreModule>();
        containerBuilder.RegisterModule(new DefaultCoreModule());
      })
      .Build();

    _serviceProvider = host.Services;
  }

  [Fact]
  public void GivenServiceProvider_WhenCompanyIdGenerated_ThenResultShouldSuccessful()
  {
    GivenServiceProvider_WhenStronglyTypedIdGenerated_ThenResultShouldSuccessful<CompanyId>();
  }

  [Fact]
  public void GivenServiceProvider_WhenEmployeeIdGenerated_ThenResultShouldSuccessful()
  {
    GivenServiceProvider_WhenStronglyTypedIdGenerated_ThenResultShouldSuccessful<EmployeeId>();
  }

  private void GivenServiceProvider_WhenStronglyTypedIdGenerated_ThenResultShouldSuccessful<T>()
  {
    var identityGenerator = _serviceProvider.GetRequiredService<IIdentityGenerator<T>>();
    var id = identityGenerator.Generate();
    _testOutputHelper.WriteLine("ID is {0}", id);
  }
}
