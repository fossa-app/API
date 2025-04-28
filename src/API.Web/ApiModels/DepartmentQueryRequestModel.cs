namespace Fossa.API.Web.ApiModels;

public record DepartmentQueryRequestModel(
    IReadOnlyList<long>? Id,
    string? Search,
    int? PageNumber,
    int? PageSize);
