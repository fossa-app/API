using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public interface IPagingQuery<TEntity, TEntityIdentity>
  : IQuery<PageResult<TEntity>>
  where TEntity : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
{
  Page Page { get; }
}
