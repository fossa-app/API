using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Core.Messages.Queries;

public interface ITenantQuery<TEntityIdentity, out TTenantIdentity, out TResult>
  : IQuery<TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
