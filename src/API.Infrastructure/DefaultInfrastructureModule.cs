using Autofac;
using Fossa.API.Infrastructure.Messages.Queries.FusionAuthApplicationFilters;
using Module = Autofac.Module;

namespace Fossa.API.Infrastructure;

public class DefaultInfrastructureModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ActiveFusionAuthApplicationFilter>().AsSelf().InstancePerLifetimeScope();
    builder.RegisterType<OriginFusionAuthApplicationFilter>().AsSelf().InstancePerLifetimeScope();
  }
}
