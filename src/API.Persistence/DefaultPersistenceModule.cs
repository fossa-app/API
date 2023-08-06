using Autofac;
using Fossa.API.Persistence.Mongo.Repositories;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence;

public class DefaultPersistenceModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    builder
      .RegisterType<MongoDatabaseProvider>()
      .AsImplementedInterfaces()
      .SingleInstance();

    builder
      .RegisterType<MongoClientProvider>()
      .AsImplementedInterfaces()
      .SingleInstance();

    builder
      .RegisterType<CompanyMongoRepository>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyRepositoryAdapter>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<MongoUnitOfWorkFactory>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();
  }
}
