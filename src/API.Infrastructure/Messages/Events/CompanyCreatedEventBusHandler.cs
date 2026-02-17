using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;

namespace Fossa.API.Infrastructure.Messages.Events;

public class CompanyCreatedEventBusHandler : CompanyEventBusHandler<CompanyCreatedEvent, CompanyChangedProtoEvent>
{
  public CompanyCreatedEventBusHandler(IMessagePublisher messagePublisher) : base(messagePublisher)
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
