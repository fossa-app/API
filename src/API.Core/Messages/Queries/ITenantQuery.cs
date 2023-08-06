namespace Fossa.API.Core.Messages.Queries;

public interface ITenantQuery<TEntityIdentity, out TTenantIdentity, out TResult>
  : IQuery<TResult>, IAffectingTenantEntities<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
