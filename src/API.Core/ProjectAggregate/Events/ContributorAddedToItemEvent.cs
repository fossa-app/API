using Fossa.API.Core.ProjectAggregate;
using Fossa.API.SharedKernel;

namespace Fossa.API.Core.ProjectAggregate.Events;

public class ContributorAddedToItemEvent : DomainEventBase
{
  public int ContributorId { get; set; }
  public ToDoItem Item { get; set; }

  public ContributorAddedToItemEvent(ToDoItem item, int contributorId)
  {
    Item = item;
    ContributorId = contributorId;
  }
}
