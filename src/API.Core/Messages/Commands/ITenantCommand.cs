﻿namespace Fossa.API.Core.Messages.Commands;

public interface ITenantCommand<TEntityIdentity, out TTenantIdentity>
  : ICommand, IAffectingTenantEntities<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
