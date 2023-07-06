using Ardalis.GuardClauses;
using Fossa.API.Core.ProjectAggregate.Events;
using Fossa.API.SharedKernel.Interfaces;
using Fossa.API.SharedKernel;

namespace Fossa.API.Core.ProjectAggregate;

public class Project : EntityBase, IAggregateRoot
{
  public string Name { get; private set; }

  private readonly List<ToDoItem> _items = new();
  public IEnumerable<ToDoItem> Items => _items.AsReadOnly();
  public ProjectStatus Status => _items.All(i => i.IsDone) ? ProjectStatus.Complete : ProjectStatus.InProgress;

  public PriorityStatus Priority { get; }

  public Project(string name, PriorityStatus priority)
  {
    Name = Guard.Against.NullOrEmpty(name);
    Priority = priority;
  }

  public void AddItem(ToDoItem newItem)
  {
    Guard.Against.Null(newItem);
    _items.Add(newItem);

    var newItemAddedEvent = new NewItemAddedEvent(this, newItem);
    RegisterDomainEvent(newItemAddedEvent);
  }

  public void UpdateName(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName);
  }
}
