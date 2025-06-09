using Fossa.API.Core.Extensions;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class CompanySettingsDeletionCommandHandler : IRequestHandler<CompanySettingsDeletionCommand, Unit>
{
  private readonly ICompanySettingsRepository _companySettingsRepository;
  private readonly ICompanySettingsQueryRepository _companySettingsQueryRepository;
  private readonly IRelationshipGraph _relationshipGraph;

  public CompanySettingsDeletionCommandHandler(
    ICompanySettingsRepository companySettingsRepository,
    ICompanySettingsQueryRepository companySettingsQueryRepository,
    IRelationshipGraph relationshipGraph)
  {
    _companySettingsRepository = companySettingsRepository ?? throw new ArgumentNullException(nameof(companySettingsRepository));
    _companySettingsQueryRepository = companySettingsQueryRepository ?? throw new ArgumentNullException(nameof(companySettingsQueryRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
  }

  public async Task<Unit> Handle(
    CompanySettingsDeletionCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _companySettingsQueryRepository.GetAsync(request.ID, cancellationToken)
      .ConfigureAwait(false);

    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _companySettingsRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

    return Unit.Value;
  }
}
