using System.Reflection;
using Autofac;
using Fossa.API.Core;
using MediatR;
using MediatR.Pipeline;
using Module = Autofac.Module;

namespace Fossa.API.Infrastructure;

public class DefaultInfrastructureModule : Module
{
  private readonly List<Assembly> _assemblies = new List<Assembly>();
  private readonly bool _isDevelopment = false;

  public DefaultInfrastructureModule(bool isDevelopment, Assembly? callingAssembly = null)
  {
    _isDevelopment = isDevelopment;
    var coreAssembly =
      Assembly.GetAssembly(typeof(DefaultCoreModule));
    var infrastructureAssembly = Assembly.GetAssembly(typeof(DefaultInfrastructureModule));
    if (coreAssembly != null)
    {
      _assemblies.Add(coreAssembly);
    }

    if (infrastructureAssembly != null)
    {
      _assemblies.Add(infrastructureAssembly);
    }

    if (callingAssembly != null)
    {
      _assemblies.Add(callingAssembly);
    }
  }

  protected override void Load(ContainerBuilder builder)
  {
    if (_isDevelopment)
    {
      RegisterDevelopmentOnlyDependencies(builder);
    }
    else
    {
      RegisterProductionOnlyDependencies(builder);
    }

    RegisterCommonDependencies(builder);
  }

  private void RegisterCommonDependencies(ContainerBuilder builder)
  {
    builder
      .RegisterType<Mediator>()
      .As<IMediator>()
      .InstancePerLifetimeScope();

    var mediatrOpenTypes = new[]
    {
      typeof(IRequestHandler<,>),
      typeof(IRequestExceptionHandler<,,>),
      typeof(IRequestExceptionAction<,>),
      typeof(INotificationHandler<>),
    };

    foreach (var mediatrOpenType in mediatrOpenTypes)
    {
      builder
        .RegisterAssemblyTypes(_assemblies.ToArray())
        .AsClosedTypesOf(mediatrOpenType)
        .AsImplementedInterfaces();
    }
  }

  private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
  {
    // Register services
  }

  private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
  {
    // Register services
  }
}
