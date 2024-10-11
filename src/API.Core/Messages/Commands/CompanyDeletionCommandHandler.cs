using Fossa.API.Core.Extensions;
using Fossa.API.Core.Relationship;
using Fossa.API.Core.Repositories;
using MediatR;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyDeletionCommandHandler : IRequestHandler<CompanyDeletionCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;
  private readonly IRelationshipGraph _relationshipGraph;

  public CompanyDeletionCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository,
    IRelationshipGraph relationshipGraph)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    _relationshipGraph = relationshipGraph ?? throw new ArgumentNullException(nameof(relationshipGraph));
  }

  public async Task<Unit> Handle(
    CompanyDeletionCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);
    await _relationshipGraph.ThrowIfHasDependentEntitiesAsync(entity.ID, cancellationToken).ConfigureAwait(false);
    await _companyRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
