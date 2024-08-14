﻿using Fossa.API.Core.Services;
using HealthChecks.MongoDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class SystemLicenseHealthCheckBuilderExtensions
{
  private const string NAME = "systemlicense";

  public static IHealthChecksBuilder AddSystemLicense(
    this IHealthChecksBuilder builder,
    string? name = default,
    HealthStatus? failureStatus = default,
    IEnumerable<string>? tags = default,
    TimeSpan? timeout = default)
  {
    return builder.Add(new HealthCheckRegistration(
        name ?? NAME,
        sp => new SystemLicenseHealthCheck(sp.GetRequiredService<ISystemLicenseRetriever>()),
        failureStatus,
        tags,
        timeout));
  }
}
