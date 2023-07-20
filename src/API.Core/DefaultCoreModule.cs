using Autofac;
using Fossa.API.Core.Identity;

namespace Fossa.API.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<IdGenIdentityGenerator>()
      .As<IIdentityGenerator<long>>()
      .SingleInstance();
  }
}
