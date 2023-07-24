using Autofac;
using Fossa.API.Core.User;
using Fossa.API.Web.Claims;

namespace Fossa.API.Web;

public class DefaultWebModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<ClaimsProvider>()
      .As<IUserIdProvider<Guid>>()
      .InstancePerLifetimeScope();
  }
}
