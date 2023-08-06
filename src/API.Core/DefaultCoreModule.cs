using Autofac;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Services;

namespace Fossa.API.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<TenantBareEntityResolver<CompanyEntity, long, Guid>>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();
  }
}
