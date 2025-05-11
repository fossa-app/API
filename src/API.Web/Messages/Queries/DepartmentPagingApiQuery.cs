using Fossa.API.Core.Messages.Queries;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.Web.Messages.Queries;

public record DepartmentPagingApiQuery(
    IReadOnlyList<long>? Id,
    string? Search,
    int? PageNumber,
    int? PageSize) : IQuery<PagingResponseModel<DepartmentRetrievalModel>>;
