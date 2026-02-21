using System.Diagnostics;
using System.Diagnostics.Metrics;
using Fossa.API.Core.Messages.Events;
using Fossa.Messaging;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Fossa.API.Infrastructure.Messages.Events;

public abstract partial class CompanyEventBusHandler<TEvent, TProtoEvent> : INotificationHandler<TEvent>
  where TEvent : ICompanyEvent<Guid>
  where TProtoEvent : IMessage
{
  private static readonly ActivitySource _activitySource = new("Fossa.API.Infrastructure");
  private readonly Counter<long> _errorCounter;
  private readonly IMessagePublisher _messagePublisher;
  private readonly ILogger<CompanyEventBusHandler<TEvent, TProtoEvent>> _logger;

  protected CompanyEventBusHandler(
      IMessagePublisher messagePublisher,
      IMeterFactory meterFactory,
      ILogger<CompanyEventBusHandler<TEvent, TProtoEvent>> logger)
  {
    _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
    var meter = (meterFactory ?? throw new ArgumentNullException(nameof(meterFactory))).Create("Fossa.API.Infrastructure");
    _errorCounter = meter.CreateCounter<long>("fossa.api.infrastructure.company_event_bus_handler.error_count");
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
    var eventType = typeof(TEvent).Name;
    var eventHandlerType = GetType().Name;

    using var activity = _activitySource.StartActivity(
        "Handle Company Event",
        ActivityKind.Internal,
        parentContext: default,
        tags: new Dictionary<string, object?>
        {
            { "company_id", companyId },
            { "entity_name", entityName },
            { "entity_id", entityId },
            { "event_type", eventType },
            { "event_handler_type", eventHandlerType }
        });

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
      _errorCounter.Add(1,
          new KeyValuePair<string, object?>("company_id", companyId),
          new KeyValuePair<string, object?>("entity_name", entityName),
          new KeyValuePair<string, object?>("entity_id", entityId),
          new KeyValuePair<string, object?>("event_type", eventType),
          new KeyValuePair<string, object?>("event_handler_type", eventHandlerType));

      LogHandlerError(_logger, companyId, entityName, entityId, ex);
    }
  }

  [LoggerMessage(EventId = 67250516, Level = LogLevel.Error, Message = "An error occurred while handling event for CompanyId: {CompanyId}, EntityName: {EntityName}, EntityId: {EntityId}")]
  private static partial void LogHandlerError(ILogger logger, long companyId, string entityName, long entityId, Exception ex);

  protected abstract TProtoEvent Map(TEvent domainEvent);

  protected abstract long ResolveEntityId(TEvent domainEvent);

  protected abstract string ResolveEntityName(TEvent domainEvent);
}
