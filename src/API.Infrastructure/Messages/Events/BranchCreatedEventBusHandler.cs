using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class BranchCreatedEventBusHandler : CompanyEventBusHandler<BranchCreatedEvent, BranchChangedProtoEvent>
{
  public BranchCreatedEventBusHandler(IMessagePublisher messagePublisher, ILogger<BranchCreatedEventBusHandler> logger) : base(messagePublisher, logger)
  {
  }

  protected override BranchChangedProtoEvent Map(BranchCreatedEvent domainEvent)
    => new()
    {
      BranchId = domainEvent.BranchId.AsPrimitive(),
      CompanyId = domainEvent.CompanyId.AsPrimitive(),
      Name = domainEvent.Name,
      Address = domainEvent.Address.Match(s => new Address
      {
        Line1 = s.Line1,
        Line2 = s.Line2.IfNone(string.Empty),
        City = s.City,
        Subdivision = s.Subdivision,
        PostalCode = s.PostalCode,
        CountryCode = s.Country.TwoLetterISORegionName
      }, default(Address)),
      TimeZoneId = domainEvent.TimeZone.Id,
    };

  protected override long ResolveEntityId(BranchCreatedEvent domainEvent)
    => domainEvent.BranchId.AsPrimitive();

  protected override string ResolveEntityName(BranchCreatedEvent domainEvent)
    => BranchEntityName;
}
