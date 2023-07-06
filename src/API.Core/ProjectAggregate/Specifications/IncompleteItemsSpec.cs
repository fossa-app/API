using Ardalis.Specification;
using Fossa.API.Core.ProjectAggregate;

namespace Fossa.API.Core.ProjectAggregate.Specifications;

public class IncompleteItemsSpec : Specification<ToDoItem>
{
  public IncompleteItemsSpec()
  {
    Query.Where(item => !item.IsDone);
  }
}
