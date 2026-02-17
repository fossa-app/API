using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public abstract partial class CompanyEventBusHandler<TEvent, TProtoEvent> : INotificationHandler<TEvent>
  where TEvent : ICompanyEvent<Guid>
  where TProtoEvent : IMessage
{
  private readonly IMessagePublisher _messagePublisher;
  private readonly ILogger<CompanyEventBusHandler<TEvent, TProtoEvent>> _logger;

  protected CompanyEventBusHandler(IMessagePublisher messagePublisher, ILogger<CompanyEventBusHandler<TEvent, TProtoEvent>> logger)
  {
    _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  protected static string BranchEntityName => "Branch";
  protected static string CompanyEntityName => "Company";
  protected static string DepartmentEntityName => "Department";
  protected static string EmployeeEntityName => "Employee";

  public async Task Handle(TEvent notification, CancellationToken cancellationToken)
  {
    var entityName = ResolveEntityName(notification);
    var entityId = ResolveEntityId(notification);
    var companyId = notification.CompanyId.AsPrimitive();

    try
    {
      var protoEvent = Map(notification);
      await _messagePublisher.PublishAsync(
        protoEvent,
        companyId,
        entityName,
        entityId,
        cancellationToken);
    }
    catch (Exception ex)
    {
      LogHandlerError(_logger, companyId, entityName, entityId, ex);
    }
  }

  [LoggerMessage(EventId = 67250516, Level = LogLevel.Error, Message = "An error occurred while handling event for CompanyId: {CompanyId}, EntityName: {EntityName}, EntityId: {EntityId}")]
  private static partial void LogHandlerError(ILogger logger, long companyId, string entityName, long entityId, Exception ex);

  protected abstract TProtoEvent Map(TEvent domainEvent);

  protected abstract long ResolveEntityId(TEvent domainEvent);

  protected abstract string ResolveEntityName(TEvent domainEvent);
}
