using Autofac;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Repositories;

namespace Fossa.API.Persistence;

public class DefaultPersistenceModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<CompanyMongoRepository>()
      .As<ICompanyMongoRepository>()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyRepositoryAdapter>()
      .As<ICompanyRepository>()
      .As<ICompanyQueryRepository>()
      .InstancePerLifetimeScope();
  }
}
