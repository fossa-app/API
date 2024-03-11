using TIKSN.Data;

namespace Fossa.API.Core.Messages.Queries;

public interface IPagingQuery
{
  Page Page { get; }
}

public interface IPagingQuery<TEntity>
  : IQuery<PageResult<TEntity>>, IPagingQuery;
