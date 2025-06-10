using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class CompanySettingsRetrievalQueryHandler : IRequestHandler<CompanySettingsRetrievalQuery, CompanySettingsEntity>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanySettingsQueryRepository _companySettingsQueryRepository;

  public CompanySettingsRetrievalQueryHandler(
    ICompanySettingsQueryRepository companySettingsQueryRepository,
    ICompanyQueryRepository companyQueryRepository)
  {
    _companySettingsQueryRepository = companySettingsQueryRepository ?? throw new ArgumentNullException(nameof(companySettingsQueryRepository));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
  }

  public async Task<CompanySettingsEntity> Handle(
    CompanySettingsRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    var companyEntity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);

    var companySettings = await _companySettingsQueryRepository.GetByCompanyIdAsync(companyEntity.ID, cancellationToken)
      .ConfigureAwait(false);

    return companySettings;
  }
}
