using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Core.Messages.Queries;

public interface ITenantQuery<out TTenantIdentity, out TResult>
  : IQuery<TResult>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
  TTenantIdentity TenantID { get; }
}
