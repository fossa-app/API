using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class CompanyUpdatedEventBusHandler : CompanyEventBusHandler<CompanyUpdatedEvent, CompanyChangedProtoEvent>
{
  public CompanyUpdatedEventBusHandler(IMessagePublisher messagePublisher, ILogger<CompanyUpdatedEventBusHandler> logger) : base(messagePublisher, logger)
  {
  }

  protected override CompanyChangedProtoEvent Map(CompanyUpdatedEvent domainEvent)
    => new()
    {
      CompanyId = domainEvent.CompanyId.AsPrimitive(),
      Name = domainEvent.Name,
      CountryCode = domainEvent.Country?.TwoLetterISORegionName
    };

  protected override long ResolveEntityId(CompanyUpdatedEvent domainEvent)
    => domainEvent.CompanyId.AsPrimitive();

  protected override string ResolveEntityName(CompanyUpdatedEvent domainEvent)
    => CompanyEntityName;
}
