namespace Fossa.API.Core.Messages.Queries;

public interface ITenantUserEntityQuery<TEntityIdentity, out TTenantIdentity, out TUserIdentity,  out TResult>
  : ITenantEntityQuery<TEntityIdentity, TTenantIdentity, TResult>
  , IUserQuery<TUserIdentity, TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;

