using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyCreationCommandHandler : IRequestHandler<CompanyCreationCommand>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;
  private readonly IIdentityGenerator<long> _identityGenerator;

  public CompanyCreationCommandHandler(
    IIdentityGenerator<long> identityGenerator,
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
  }

  public async Task Handle(
    CompanyCreationCommand request,
    CancellationToken cancellationToken)
  {
    var oldEntity = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);
    if (oldEntity.IsSome)
    {
      throw new InvalidOperationException("A company for this tenant has already been created.");
    }
    var id = _identityGenerator.Generate();
    CompanyEntity entity = new(id, request.TenantID, request.Name);
    await _companyRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
  }
}
