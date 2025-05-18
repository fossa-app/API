using Fossa.API.Web.ApiModels;

namespace Fossa.API.Web.Messages.Queries;

public record EmployeePagingApiQuery(
    IReadOnlyList<long>? Id,
    string? Search,
    int? PageNumber,
    int? PageSize) : IPagingApiQuery<EmployeeRetrievalModel>;
