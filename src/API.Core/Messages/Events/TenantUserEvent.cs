using TIKSN.Integration.Messages.Events;

namespace Fossa.API.Core.Messages.Events;

public abstract record TenantUserEvent<TTenantIdentity, TUserIdentity> : TenantEvent<TTenantIdentity>, IUserEvent<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
  protected TenantUserEvent(TTenantIdentity tenantID, TUserIdentity userID) : base(tenantID)
  {
    UserID = userID;
  }

  public TUserIdentity UserID { get; }
}
