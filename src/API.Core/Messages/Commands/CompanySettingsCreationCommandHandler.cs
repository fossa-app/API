using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using TIKSN.Data;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class CompanySettingsCreationCommandHandler : IRequestHandler<CompanySettingsCreationCommand, Unit>
{
  private readonly ICompanySettingsRepository _companySettingsRepository;
  private readonly ICompanySettingsQueryRepository _companySettingsQueryRepository;
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IIdentityGenerator<CompanySettingsId> _identityGenerator;

  public CompanySettingsCreationCommandHandler(
    ICompanySettingsRepository companySettingsRepository,
    ICompanySettingsQueryRepository companySettingsQueryRepository,
    ICompanyQueryRepository companyQueryRepository,
    IIdentityGenerator<CompanySettingsId> identityGenerator)
  {
    _companySettingsRepository = companySettingsRepository ?? throw new ArgumentNullException(nameof(companySettingsRepository));
    _companySettingsQueryRepository = companySettingsQueryRepository ?? throw new ArgumentNullException(nameof(companySettingsQueryRepository));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
  }

  public async Task<Unit> Handle(
    CompanySettingsCreationCommand request,
    CancellationToken cancellationToken)
  {
    var companyEntity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);

    var existingSettings = await _companySettingsQueryRepository.FindByCompanyIdAsync(companyEntity.ID, cancellationToken)
      .ConfigureAwait(false);

    if (existingSettings.IsSome)
    {
      throw new InvalidOperationException("Company settings for this company have already been created.");
    }

    var id = _identityGenerator.Generate();
    CompanySettingsEntity entity = new(id, companyEntity.ID, request.ColorSchemeId);
    await _companySettingsRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

    return Unit.Value;
  }
}
