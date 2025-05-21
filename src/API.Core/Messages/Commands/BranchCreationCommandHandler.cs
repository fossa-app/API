using Fossa.API.Core.Entities;
using Fossa.API.Core.Extensions;
using Fossa.API.Core.Licensing;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using Fossa.Licensing;
using TIKSN.Identity;
using TIKSN.Licensing;

namespace Fossa.API.Core.Messages.Commands;

public class BranchCreationCommandHandler : IRequestHandler<BranchCreationCommand, Unit>
{
  private readonly IBranchRepository _branchRepository;
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IIdentityGenerator<BranchId> _identityGenerator;
  private readonly IPostalCodeParser _postalCodeParser;
  private readonly IBranchQueryRepository _branchQueryRepository;
  private readonly ICompanyLicenseRetriever _companyLicenseRetriever;

  public BranchCreationCommandHandler(
    IIdentityGenerator<BranchId> identityGenerator,
    ICompanyQueryRepository companyQueryRepository,
    IBranchRepository branchRepository,
    IPostalCodeParser postalCodeParser,
    IBranchQueryRepository branchQueryRepository,
    ICompanyLicenseRetriever companyLicenseRetriever)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
    _postalCodeParser = postalCodeParser ?? throw new ArgumentNullException(nameof(postalCodeParser));
    _branchQueryRepository = branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
    _companyLicenseRetriever = companyLicenseRetriever ?? throw new ArgumentNullException(nameof(companyLicenseRetriever));
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
    await ValidateEntitlementsAsync(companyEntity.ID, cancellationToken).ConfigureAwait(false);
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

  private static bool EnsureMaximumBranchCountWillNotExceed(
    int maximumBranchCount, int currentBranchCount)
  {
    return maximumBranchCount > currentBranchCount;
  }

  private async Task ValidateEntitlementsAsync(CompanyId companyId, CancellationToken cancellationToken)
  {
    var licenseValidation = await _companyLicenseRetriever.GetAsync(companyId, cancellationToken).ConfigureAwait(false);

    var currentBranchCount = await _branchQueryRepository.CountAllAsync(companyId, cancellationToken).ConfigureAwait(false);

    _ = licenseValidation.Match(
        license =>
          Success<Error, License<CompanyEntitlements>>(license)
            .Validate(
              x => EnsureMaximumBranchCountWillNotExceed(x.Entitlements.MaximumBranchCount, currentBranchCount),
                43705652,
                "The current company license entitlements limit the number of branches that can be created, and this limit has been reached")
              .Map(_ => unit),
        _ =>
          EnsureMaximumBranchCountWillNotExceed(UnlicensedCompanyEntitlements.MaximumBranchCount, currentBranchCount)
            ? Success<Error, LanguageExt.Unit>(unit)
            : Fail<Error, LanguageExt.Unit>(Error.New(
              43722467,
              "The current company is unlicensed and the maximum number of branches that can be created has been reached. Please contact your system administrator to obtain a license for this company.")))
      .GetOrThrow();
  }
}
