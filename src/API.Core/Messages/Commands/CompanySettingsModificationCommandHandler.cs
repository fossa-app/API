using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Messages.Commands;

public class CompanySettingsModificationCommandHandler : IRequestHandler<CompanySettingsModificationCommand, Unit>
{
  private readonly ICompanySettingsRepository _companySettingsRepository;
  private readonly ICompanySettingsQueryRepository _companySettingsQueryRepository;

  public CompanySettingsModificationCommandHandler(
    ICompanySettingsRepository companySettingsRepository,
    ICompanySettingsQueryRepository companySettingsQueryRepository)
  {
    _companySettingsRepository = companySettingsRepository ?? throw new ArgumentNullException(nameof(companySettingsRepository));
    _companySettingsQueryRepository = companySettingsQueryRepository ?? throw new ArgumentNullException(nameof(companySettingsQueryRepository));
  }

  public async Task<Unit> Handle(
    CompanySettingsModificationCommand request,
    CancellationToken cancellationToken)
  {
    var existingEntity = await _companySettingsQueryRepository.GetAsync(request.ID, cancellationToken)
      .ConfigureAwait(false);

    var updatedEntity = existingEntity with { ColorSchemeId = request.ColorSchemeId };
    await _companySettingsRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

    return Unit.Value;
  }
}
