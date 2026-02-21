using System.Diagnostics.Metrics;
using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class BranchUpdatedEventBusHandler : CompanyEventBusHandler<BranchUpdatedEvent, BranchChangedProtoEvent>
{
  public BranchUpdatedEventBusHandler(
      IMessagePublisher messagePublisher,
      IMeterFactory meterFactory,
      ILogger<BranchUpdatedEventBusHandler> logger) : base(messagePublisher, meterFactory, logger)
  {
  }

  protected override BranchChangedProtoEvent Map(BranchUpdatedEvent domainEvent)
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

  protected override long ResolveEntityId(BranchUpdatedEvent domainEvent)
    => domainEvent.BranchId.AsPrimitive();

  protected override string ResolveEntityName(BranchUpdatedEvent domainEvent)
    => BranchEntityName;
}
