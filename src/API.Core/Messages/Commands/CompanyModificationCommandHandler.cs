using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyModificationCommandHandler : IRequestHandler<CompanyModificationCommand>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;

  public CompanyModificationCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
  }

  public async Task Handle(
    CompanyModificationCommand request,
    CancellationToken cancellationToken)
  {
    CompanyEntity entity = await _companyQueryRepository.GetOrDefaultAsync(request.ID, cancellationToken).ConfigureAwait(false);
    entity = entity with { Name = request.Name };
    await _companyRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
  }
}
