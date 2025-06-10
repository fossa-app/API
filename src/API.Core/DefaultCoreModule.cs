using Autofac;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Services;
using Fossa.API.Core.TimeZone;
using TIKSN.Data.BareEntityResolvers;
using TIKSN.Identity;

namespace Fossa.API.Core;

public class DefaultCoreModule : Module
{
#pragma warning disable MA0051 // Method is too long
  protected override void Load(ContainerBuilder builder)
#pragma warning restore MA0051 // Method is too long
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
      .RegisterType<CompanyLicenseRetriever>()
      .AsImplementedInterfaces()
      .InstancePerLifetimeScope();

    builder
      .RegisterType<CompanyLicenseCreator>()
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

    builder
      .RegisterType<DateTimeZoneLookup>()
      .AsImplementedInterfaces()
      .SingleInstance();

    builder
      .RegisterType<DateTimeZoneProvider>()
      .AsImplementedInterfaces()
      .SingleInstance();

    builder
      .RegisterType<CountryProvider>()
      .AsImplementedInterfaces()
      .SingleInstance();

    builder
      .RegisterType<PostalCodeParser>()
      .AsImplementedInterfaces()
      .SingleInstance();
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
    RegisterStronglyTypedId<long, CompanySettingsId>(builder);
    RegisterStronglyTypedId<long, EmployeeId>(builder);
    RegisterStronglyTypedId<long, BranchId>(builder);
    RegisterStronglyTypedId<long, DepartmentId>(builder);
  }
}
