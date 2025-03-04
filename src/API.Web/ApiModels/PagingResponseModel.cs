namespace Fossa.API.Web.ApiModels;

public record PagingResponseModel<T>(
  int? PageNumber, int? PageSize,
  IReadOnlyCollection<T> Items,
  long? TotalItems,
  long? TotalPages);
