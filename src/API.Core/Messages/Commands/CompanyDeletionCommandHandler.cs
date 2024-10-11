using Fossa.API.Core.Repositories;
using MediatR;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyDeletionCommandHandler : IRequestHandler<CompanyDeletionCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;

  public CompanyDeletionCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
  }

  public async Task<Unit> Handle(
    CompanyDeletionCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);
    await _companyRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
