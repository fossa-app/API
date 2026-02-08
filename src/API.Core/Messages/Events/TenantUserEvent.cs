using MediatR;
using TIKSN.Integration.Messages.Events;

namespace Fossa.API.Core.Messages.Events;

public abstract record TenantUserEvent<TUserIdentity, TTenantIdentity> : TenantEvent<TTenantIdentity>, IUserEvent<TUserIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  protected TenantUserEvent(TTenantIdentity tenantID, TUserIdentity userID) : base(tenantID)
  {
    UserID = userID;
  }

  public TUserIdentity UserID { get; }
}
