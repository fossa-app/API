using Fossa.API.Web.ApiModels;
using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Web.Messages.Queries;

public interface IPagingApiQuery
{
  int? PageNumber { get; }
  int? PageSize { get; }
}

public interface IPagingApiQuery<TRetrievalModel> : IQuery<PagingResponseModel<TRetrievalModel>>, IPagingApiQuery;
