namespace Fossa.API.Core.Messages.Queries;

public interface ITenantUserQuery<out TUserIdentity, out TTenantIdentity, out TResult>
  : ITenantQuery<TTenantIdentity, TResult>
{
  TUserIdentity UserID { get; }
}
