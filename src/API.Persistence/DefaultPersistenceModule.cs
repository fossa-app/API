using Autofac;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Repositories;
using TIKSN.Data;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence;

public class DefaultPersistenceModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<MongoDatabaseProvider>()
      .As<IMongoDatabaseProvider>()
      .SingleInstance();

    builder
      .RegisterType<MongoClientProvider>()
      .As<IMongoClientProvider>()
      .SingleInstance();

    builder
      .RegisterType<CompanyMongoRepository>()
      .As<ICompanyMongoRepository>()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyRepositoryAdapter>()
      .As<ICompanyRepository>()
      .As<ICompanyQueryRepository>()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<MongoUnitOfWorkFactory>()
      .As<IUnitOfWorkFactory>()
      .InstancePerLifetimeScope();
  }
}
