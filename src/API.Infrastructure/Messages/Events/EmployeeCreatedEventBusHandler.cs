using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class EmployeeCreatedEventBusHandler : CompanyEventBusHandler<EmployeeCreatedEvent, EmployeeChangedProtoEvent>
{
  public EmployeeCreatedEventBusHandler(IMessagePublisher messagePublisher, ILogger<EmployeeCreatedEventBusHandler> logger) : base(messagePublisher, logger)
  {
  }

  protected override EmployeeChangedProtoEvent Map(EmployeeCreatedEvent domainEvent)
    => new()
    {
      EmployeeId = domainEvent.EmployeeId.AsPrimitive(),
      CompanyId = domainEvent.CompanyId.AsPrimitive(),
      FirstName = domainEvent.FirstName,
      LastName = domainEvent.LastName,
      FullName = domainEvent.FullName
    };

  protected override long ResolveEntityId(EmployeeCreatedEvent domainEvent)
    => domainEvent.EmployeeId.AsPrimitive();

  protected override string ResolveEntityName(EmployeeCreatedEvent domainEvent)
    => EmployeeEntityName;
}
