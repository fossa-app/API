using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class CompanySettingsDeletionCommandHandler : IRequestHandler<CompanySettingsDeletionCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanySettingsQueryRepository _companySettingsQueryRepository;
  private readonly ICompanySettingsRepository _companySettingsRepository;
  private readonly IRelationshipGraph _relationshipGraph;
  private readonly IPublisher _publisher;

  public CompanySettingsDeletionCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanySettingsRepository companySettingsRepository,
    ICompanySettingsQueryRepository companySettingsQueryRepository,
    IRelationshipGraph relationshipGraph,
    IPublisher publisher)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companySettingsRepository = companySettingsRepository ?? throw new ArgumentNullException(nameof(companySettingsRepository));
    _companySettingsQueryRepository = companySettingsQueryRepository ?? throw new ArgumentNullException(nameof(companySettingsQueryRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
    CompanySettingsDeletionCommand request,
    CancellationToken cancellationToken)
  {
    var compamyEntity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);

    var entity = await _companySettingsQueryRepository.GetByCompanyIdAsync(compamyEntity.ID, cancellationToken)
      .ConfigureAwait(false);

    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _companySettingsRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

    var deletedEvent = new CompanySettingsDeletedEvent(
      compamyEntity.TenantID,
      entity.ID,
      entity.CompanyId);

    await _publisher.Publish(deletedEvent, cancellationToken).ConfigureAwait(false);

    return Unit.Value;
  }
}
