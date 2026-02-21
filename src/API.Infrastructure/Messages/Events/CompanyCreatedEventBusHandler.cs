using System.Diagnostics.Metrics;
using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class CompanyCreatedEventBusHandler : CompanyEventBusHandler<CompanyCreatedEvent, CompanyChangedProtoEvent>
{
  public CompanyCreatedEventBusHandler(
      IMessagePublisher messagePublisher,
      IMeterFactory meterFactory,
      ILogger<CompanyCreatedEventBusHandler> logger) : base(messagePublisher, meterFactory, logger)
  {
  }

  protected override CompanyChangedProtoEvent Map(CompanyCreatedEvent domainEvent)
    => new()
    {
      CompanyId = domainEvent.CompanyId.AsPrimitive(),
      Name = domainEvent.Name,
      CountryCode = domainEvent.Country?.TwoLetterISORegionName
    };

  protected override long ResolveEntityId(CompanyCreatedEvent domainEvent)
    => domainEvent.CompanyId.AsPrimitive();

  protected override string ResolveEntityName(CompanyCreatedEvent domainEvent)
    => CompanyEntityName;
}
