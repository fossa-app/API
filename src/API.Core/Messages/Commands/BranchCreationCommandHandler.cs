using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class BranchCreationCommandHandler : IRequestHandler<BranchCreationCommand, Unit>
{
  private readonly IBranchRepository _branchRepository;
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IIdentityGenerator<BranchId> _identityGenerator;
  private readonly IPostalCodeParser _postalCodeParser;

  public BranchCreationCommandHandler(
    IIdentityGenerator<BranchId> identityGenerator,
    ICompanyQueryRepository companyQueryRepository,
    IBranchRepository branchRepository,
    IPostalCodeParser postalCodeParser)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
    _postalCodeParser = postalCodeParser ?? throw new ArgumentNullException(nameof(postalCodeParser));
  }

  public async Task<Unit> Handle(
    BranchCreationCommand request,
    CancellationToken cancellationToken)
  {
    var companyEntity = await _companyQueryRepository.FindByTenantIdAsync(request.TenantID, cancellationToken)
      .ConfigureAwait(false);

    await companyEntity.Match(
        s => CreateBranchAsync(s, request, cancellationToken),
        () => throw new FailedDependencyException("A company for this tenant have not been created."))
      .ConfigureAwait(false);
    return Unit.Value;
  }

  private async Task CreateBranchAsync(
    CompanyEntity companyEntity,
    BranchCreationCommand request,
    CancellationToken cancellationToken)
  {
    var id = _identityGenerator.Generate();
    BranchEntity entity = new(
      id,
      request.TenantID,
      companyEntity.ID,
      request.Name,
      request.TimeZone,
      request.Address.Map(x =>
      {
        var postalCode = _postalCodeParser.ParsePostalCode(x.Country, x.PostalCode).GetOrThrow();
        return x with { PostalCode = postalCode };
      }));

    await _branchRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
  }
}
