using Autofac;
using Fossa.API.Web.Claims;
using Fossa.API.Web.Mappers;

namespace Fossa.API.Web;

public class DefaultWebModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<ClaimsProvider>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterGeneric(typeof(PagingResponseModelMapper<,>))
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();
  }
}
