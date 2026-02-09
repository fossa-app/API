using MediatR;
using TIKSN.Integration.Messages.Events;

namespace Fossa.API.Core.Messages.Events;

public abstract record TenantEvent<TTenantIdentity> : ITenantEvent<TTenantIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  protected TenantEvent(TTenantIdentity tenantID)
  {
    TenantID = tenantID;
  }

  public TTenantIdentity TenantID { get; }
}
