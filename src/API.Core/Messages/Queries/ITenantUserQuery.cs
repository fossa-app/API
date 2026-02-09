namespace Fossa.API.Core.Messages.Queries;

public interface ITenantUserQuery<out TTenantIdentity, out TUserIdentity, out TResult>
  : ITenantQuery<TTenantIdentity, TResult>
  , IUserQuery<TUserIdentity, TResult>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;

