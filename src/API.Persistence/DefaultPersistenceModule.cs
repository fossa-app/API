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
      .RegisterType<MongoUnitOfWorkFactory>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyMongoRepository>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyRepositoryAdapter>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<BranchMongoRepository>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<BranchRepositoryAdapter>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<EmployeeMongoRepository>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<EmployeeRepositoryAdapter>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<SystemPropertiesMongoRepository>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<SystemPropertiesRepositoryAdapter>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<LicenseMongoFileRepository>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();
  }
}
