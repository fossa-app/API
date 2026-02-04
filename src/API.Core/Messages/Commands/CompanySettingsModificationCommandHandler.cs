using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class CompanySettingsModificationCommandHandler : IRequestHandler<CompanySettingsModificationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanySettingsQueryRepository _companySettingsQueryRepository;
  private readonly ICompanySettingsRepository _companySettingsRepository;
  private readonly IPublisher _publisher;

  public CompanySettingsModificationCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanySettingsRepository companySettingsRepository,
    ICompanySettingsQueryRepository companySettingsQueryRepository,
    IPublisher publisher)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companySettingsRepository = companySettingsRepository ?? throw new ArgumentNullException(nameof(companySettingsRepository));
    _companySettingsQueryRepository = companySettingsQueryRepository ?? throw new ArgumentNullException(nameof(companySettingsQueryRepository));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
    CompanySettingsModificationCommand request,
    CancellationToken cancellationToken)
  {
    var companyEntity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);

    var existingEntity = await _companySettingsQueryRepository.GetByCompanyIdAsync(companyEntity.ID, cancellationToken)
      .ConfigureAwait(false);

    var updatedEntity = existingEntity with { ColorSchemeId = request.ColorSchemeId };
    await _companySettingsRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

    var updatedEvent = new CompanySettingsUpdatedEvent(
      companyEntity.TenantID,
      updatedEntity.ID,
      updatedEntity.CompanyId,
      updatedEntity.ColorSchemeId);

    await _publisher.Publish(updatedEvent, cancellationToken).ConfigureAwait(false);

    return Unit.Value;
  }
}
