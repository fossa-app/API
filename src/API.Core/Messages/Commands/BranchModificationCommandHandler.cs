using Fossa.API.Core.Extensions;
using Fossa.API.Core.Messages.Events;
using Fossa.API.Core.Repositories;
using Fossa.API.Core.Services;
using TIKSN.Parsing;

namespace Fossa.API.Core.Messages.Commands;

public class BranchModificationCommandHandler : IRequestHandler<BranchModificationCommand, Unit>
{
  private readonly IBranchQueryRepository _branchQueryRepository;
  private readonly IBranchRepository _branchRepository;
  private readonly IPostalCodeParser _postalCodeParser;
  private readonly IPublisher _publisher;

  public BranchModificationCommandHandler(
    IBranchQueryRepository branchQueryRepository,
    IBranchRepository branchRepository,
    IPostalCodeParser postalCodeParser,
    IPublisher publisher)
  {
    _branchQueryRepository = branchQueryRepository ?? throw new ArgumentNullException(nameof(branchQueryRepository));
    _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
    _postalCodeParser = postalCodeParser ?? throw new ArgumentNullException(nameof(postalCodeParser));
    _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
  }

  public async Task<Unit> Handle(
    BranchModificationCommand request,
    CancellationToken cancellationToken)
  {
    var originalEntity = await _branchQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);

    var entity = originalEntity with
    {
      Name = request.Name,
      TimeZone = request.TimeZone,
      Address = request.Address.Map(x =>
      {
        var postalCode = _postalCodeParser.ParsePostalCode(x.Country, x.PostalCode).GetOrThrow();
        return x with { PostalCode = postalCode };
      }),
    };
    await _branchRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);

    var updatedEvent = new BranchUpdatedEvent(
      entity.TenantID,
      entity.ID,
      entity.CompanyId,
      entity.Name,
      entity.TimeZone,
      entity.Address);

    await _publisher.Publish(updatedEvent, cancellationToken).ConfigureAwait(false);

    if (originalEntity.TimeZone != entity.TimeZone)
    {
      var timeZoneChangedEvent = new BranchTimeZoneChangedEvent(
        entity.TenantID,
        entity.ID,
        entity.CompanyId,
        originalEntity.TimeZone,
        entity.TimeZone);

      await _publisher.Publish(timeZoneChangedEvent, cancellationToken).ConfigureAwait(false);
    }

    if (originalEntity.Address != entity.Address)
    {
      var addressChangedEvent = new BranchAddressChangedEvent(
        entity.TenantID,
        entity.ID,
        entity.CompanyId,
        originalEntity.Address,
        entity.Address);

      await _publisher.Publish(addressChangedEvent, cancellationToken).ConfigureAwait(false);
    }

    return Unit.Value;
  }
}
