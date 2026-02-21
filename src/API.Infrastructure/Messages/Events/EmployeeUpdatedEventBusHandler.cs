using System.Diagnostics.Metrics;
using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Fossa.Messaging.Messages.Events;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public class EmployeeUpdatedEventBusHandler : CompanyEventBusHandler<EmployeeUpdatedEvent, EmployeeChangedProtoEvent>
{
  public EmployeeUpdatedEventBusHandler(
      IMessagePublisher messagePublisher,
      IMeterFactory meterFactory,
      ILogger<EmployeeUpdatedEventBusHandler> logger) : base(messagePublisher, meterFactory, logger)
  {
  }

  protected override EmployeeChangedProtoEvent Map(EmployeeUpdatedEvent domainEvent)
    => new()
    {
      EmployeeId = domainEvent.EmployeeId.AsPrimitive(),
      CompanyId = domainEvent.CompanyId.AsPrimitive(),
      FirstName = domainEvent.FirstName,
      LastName = domainEvent.LastName,
      FullName = domainEvent.FullName,
      JobTitle = domainEvent.JobTitle,
      AssignedBranchId = domainEvent.AssignedBranchId.Match(s => s.AsPrimitive(), default(long)),
      AssignedDepartmentId = domainEvent.AssignedDepartmentId.Match(s => s.AsPrimitive(), default(long)),
      ReportsToId = domainEvent.ReportsToId.Match(s => s.AsPrimitive(), default(long))
    };

  protected override long ResolveEntityId(EmployeeUpdatedEvent domainEvent)
    => domainEvent.EmployeeId.AsPrimitive();

  protected override string ResolveEntityName(EmployeeUpdatedEvent domainEvent)
    => EmployeeEntityName;
}
