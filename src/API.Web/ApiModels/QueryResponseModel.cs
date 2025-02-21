namespace Fossa.API.Web.ApiModels;

public record QueryResponseModel<T>(
  IEnumerable<T>? List,
  PagingResponseModel<T>? Page);
