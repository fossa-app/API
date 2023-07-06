﻿using Ardalis.GuardClauses;
using Fossa.API.SharedKernel.Interfaces;
using Fossa.API.SharedKernel;

namespace Fossa.API.Core.ContributorAggregate;

public class Contributor : EntityBase, IAggregateRoot
{
  public string Name { get; private set; }

  public Contributor(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
  }

  public void UpdateName(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
  }
}
