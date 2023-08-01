namespace Fossa.API.Core.Messages.Queries;

public interface ITenantQuery<out TTenantIdentity, out TResult> : IQuery<TResult>
{
  TTenantIdentity TenantID { get; }
}
