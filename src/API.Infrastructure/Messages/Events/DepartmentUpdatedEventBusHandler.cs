using System.Diagnostics.Metrics;
using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class DepartmentUpdatedEventBusHandler : CompanyEventBusHandler<DepartmentUpdatedEvent, DepartmentChangedProtoEvent>
{
  public DepartmentUpdatedEventBusHandler(
      IMessagePublisher messagePublisher,
      IMeterFactory meterFactory,
      ILogger<DepartmentUpdatedEventBusHandler> logger) : base(messagePublisher, meterFactory, logger)
  {
  }

  protected override DepartmentChangedProtoEvent Map(DepartmentUpdatedEvent domainEvent)
    => new()
    {
      DepartmentId = domainEvent.DepartmentId.AsPrimitive(),
      CompanyId = domainEvent.CompanyId.AsPrimitive(),
      Name = domainEvent.Name,
      ParentDepartmentId = domainEvent.ParentDepartmentId.Match(d => d.AsPrimitive(), default(long)),
      ManagerId = domainEvent.ManagerId.AsPrimitive()
    };

  protected override long ResolveEntityId(DepartmentUpdatedEvent domainEvent)
    => domainEvent.DepartmentId.AsPrimitive();

  protected override string ResolveEntityName(DepartmentUpdatedEvent domainEvent)
    => DepartmentEntityName;
}
