namespace Fossa.API.Core.Messages.Queries;

public interface ITenantUserQuery<TEntityIdentity, out TUserIdentity, out TTenantIdentity, out TResult>
  : ITenantQuery<TEntityIdentity, TTenantIdentity, TResult>
  , IUserQuery<TUserIdentity, TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;
