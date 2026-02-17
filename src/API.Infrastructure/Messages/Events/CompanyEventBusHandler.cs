using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Google.Protobuf;

namespace Fossa.API.Infrastructure.Messages.Events;

public abstract class CompanyEventBusHandler<TEvent, TProtoEvent> : INotificationHandler<TEvent>
  where TEvent : ICompanyEvent<Guid>
  where TProtoEvent : IMessage
{
  private readonly IMessagePublisher _messagePublisher;

  protected CompanyEventBusHandler(IMessagePublisher messagePublisher)
  {
    _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
  }

  protected static string BranchEntityName => "Branch";
  protected static string CompanyEntityName => "Company";
  protected static string DepartmentEntityName => "Department";
  protected static string EmployeeEntityName => "Employee";

  public async Task Handle(TEvent notification, CancellationToken cancellationToken)
  {
    var protoEvent = Map(notification);
    await _messagePublisher.PublishAsync(
      protoEvent,
      notification.CompanyId.AsPrimitive(),
      ResolveEntityName(notification),
      ResolveEntityId(notification),
      cancellationToken);
  }

  protected abstract TProtoEvent Map(TEvent domainEvent);

  protected abstract long ResolveEntityId(TEvent domainEvent);

  protected abstract string ResolveEntityName(TEvent domainEvent);
}
