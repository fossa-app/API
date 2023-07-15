using Autofac;
using Fossa.API.Core.Identity;
using Fossa.API.Core.Interfaces;
using Fossa.API.Core.Services;

namespace Fossa.API.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder.RegisterType<ToDoItemSearchService>()
        .As<IToDoItemSearchService>().InstancePerLifetimeScope();

    builder.RegisterType<DeleteContributorService>()
        .As<IDeleteContributorService>().InstancePerLifetimeScope();

    builder
      .RegisterType<IdGenIdentityGenerator>()
      .As<IIdentityGenerator<long>>()
      .SingleInstance();
  }
}
