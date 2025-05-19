using Fossa.API.Core.Messages.Queries;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.Web.Messages.Queries;

public interface IPagingApiQuery
{
  int? PageNumber { get; }
  int? PageSize { get; }
}

public interface IPagingApiQuery<TRetrievalModel> : IQuery<PagingResponseModel<TRetrievalModel>>, IPagingApiQuery;
