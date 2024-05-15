using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Microsoft.Extensions.Logging;
using TIKSN.Data;

namespace Fossa.API.Core.Services;

public class SystemPropertiesInitializer : ISystemPropertiesInitializer
{
  private readonly ISystemPropertiesRepository _systemPropertiesRepository;
  private readonly ISystemPropertiesQueryRepository _systemPropertiesQueryRepository;
  private readonly ILogger<SystemPropertiesInitializer> _logger;

  public SystemPropertiesInitializer(
    ISystemPropertiesRepository systemPropertiesRepository,
    ISystemPropertiesQueryRepository systemPropertiesQueryRepository,
    ILogger<SystemPropertiesInitializer> logger)
  {
    _systemPropertiesRepository = systemPropertiesRepository ??
                                  throw new ArgumentNullException(nameof(systemPropertiesRepository));
    _systemPropertiesQueryRepository = systemPropertiesQueryRepository ??
                                       throw new ArgumentNullException(nameof(systemPropertiesQueryRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task InitializeAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Checking existence of system properties");
    var systemPropertiesEntity = await _systemPropertiesQueryRepository
      .GetOrNoneAsync(SystemProperties.MainSystemPropertiesId, cancellationToken)
      .ConfigureAwait(false);

    var systemProperties = await systemPropertiesEntity
      .MatchAsync(
        s => s,
        () => CreateSystemPropertiesAsync(cancellationToken))
      .ConfigureAwait(false);

    _logger.LogInformation("SystemID is {SystemID}", systemProperties.SystemID);
  }

  private async Task<SystemPropertiesEntity> CreateSystemPropertiesAsync(
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Creating system properties");

    var entity = new SystemPropertiesEntity(
      SystemProperties.MainSystemPropertiesId,
      Ulid.NewUlid());

    await _systemPropertiesRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

    _logger.LogInformation("Created system properties");
    return entity;
  }
}
