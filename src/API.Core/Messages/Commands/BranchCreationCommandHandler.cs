using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class BranchCreationCommandHandler : IRequestHandler<BranchCreationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IBranchRepository _branchRepository;
  private readonly IIdentityGenerator<BranchId> _identityGenerator;

  public BranchCreationCommandHandler(
    IIdentityGenerator<BranchId> identityGenerator,
    ICompanyQueryRepository companyQueryRepository,
    IBranchRepository branchRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
  }

  public async Task<Unit> Handle(
    BranchCreationCommand request,
    CancellationToken cancellationToken)
  {
    var companyEntity = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);

    await companyEntity.Match(
        s => CreateBranchAsync(s, request, cancellationToken),
        () => throw new InvalidOperationException("A company for this tenant have not been created."))
      .ConfigureAwait(false);
    return Unit.Value;
  }

  public async Task CreateBranchAsync(
    CompanyEntity companyEntity,
    BranchCreationCommand request,
    CancellationToken cancellationToken)
  {
    var id = _identityGenerator.Generate();
    BranchEntity entity = new(
      id,
      request.TenantID,
      request.UserID,
      companyEntity.ID,
      request.Name);

    await _branchRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
  }
}
