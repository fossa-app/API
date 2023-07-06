using Fossa.API.SharedKernel;

namespace Fossa.API.SharedKernel.Interfaces;

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEventsAsync(IEnumerable<EntityBase> entitiesWithEvents);
}
