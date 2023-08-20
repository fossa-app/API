namespace Fossa.API.Core.Messages.Queries;

public interface ITenantUserQuery<TEntityIdentity, out TUserIdentity, out TTenantIdentity, out TResult>
  : ITenantQuery<TEntityIdentity, TTenantIdentity, TResult>
  , IUserQuery<TUserIdentity, TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
}
