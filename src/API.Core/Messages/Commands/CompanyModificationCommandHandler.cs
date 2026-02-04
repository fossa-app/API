using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class CompanyModificationCommandHandler : IRequestHandler<CompanyModificationCommand, Unit>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly ICompanyRepository _companyRepository;
  private readonly IPublisher _publisher;

  public CompanyModificationCommandHandler(
    ICompanyQueryRepository companyQueryRepository,
    ICompanyRepository companyRepository,
    IPublisher publisher)
  {
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
    CompanyModificationCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _companyQueryRepository.GetByTenantIdAsync(request.TenantID, cancellationToken).ConfigureAwait(false);
    entity = entity with { Name = request.Name, Country = request.Country };
    await _companyRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);

    var updatedEvent = new CompanyUpdatedEvent(
      entity.TenantID,
      entity.ID,
      entity.Name,
      entity.Country);

    await _publisher.Publish(updatedEvent, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
