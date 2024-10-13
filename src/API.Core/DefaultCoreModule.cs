using Autofac;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Services;
using TIKSN.Data.BareEntityResolvers;
using TIKSN.Identity;

namespace Fossa.API.Core;

public class DefaultCoreModule : Module
{
  protected override void Load(ContainerBuilder builder)
  {
    RegisterStronglyTypedIds(builder);

    builder
      .RegisterType<TenantBareEntityResolver<CompanyEntity, CompanyId, Guid>>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<SystemInitializer>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<SystemPropertiesInitializer>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<SystemLicenseInitializer>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyLicenseInitializer>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<SystemLicenseRetriever>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CertificateProvider>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<RelationshipGraph>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();
  }

  private static void RegisterStronglyTypedId<TSource, TDestination>(ContainerBuilder builder)
  {
    builder
      .RegisterType<MapperIdentityGenerator<TSource, TDestination>>()
      .AsImplementedInterfaces()
      .SingleInstance();
  }

  private static void RegisterStronglyTypedIds(ContainerBuilder builder)
  {
    RegisterStronglyTypedId<long, CompanyId>(builder);
    RegisterStronglyTypedId<long, EmployeeId>(builder);
    RegisterStronglyTypedId<long, BranchId>(builder);
  }
}
