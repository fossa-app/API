using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Data;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class EmployeePagingApiQueryHandler : ApiMessageHandler<EmployeeId, EmployeePagingApiQuery, PagingResponseModel<EmployeeRetrievalModel>, EmployeeListingQuery, Seq<EmployeeEntity>, EmployeePagingQuery, PageResult<EmployeeEntity>>
{
  private readonly IMapper<Seq<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> _listingMapper;
  private readonly IMapper<PageResult<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> _pagingMapper;

  public EmployeePagingApiQueryHandler(
      IMapper<PageResult<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> pagingMapper,
      IMapper<Seq<EmployeeEntity>, PagingResponseModel<EmployeeRetrievalModel>> listingMapper,
      ISender sender,
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper) : base(
          sender,
          tenantIdProvider,
          userIdProvider,
          domainIdentityToDataIdentityMapper,
          dataIdentityToDomainIdentityMapper)
  {
    _pagingMapper = pagingMapper ?? throw new ArgumentNullException(nameof(pagingMapper));
    _listingMapper = listingMapper ?? throw new ArgumentNullException(nameof(listingMapper));
  }

  protected override PagingResponseModel<EmployeeRetrievalModel> MapToApiResponse(PageResult<EmployeeEntity> domainResponse)
  {
    return _pagingMapper.Map(domainResponse);
  }

  protected override PagingResponseModel<EmployeeRetrievalModel> MapToApiResponse(Seq<EmployeeEntity> domainResponse)
  {
    return _listingMapper.Map(domainResponse);
  }

  protected override Either<EmployeeListingQuery, EmployeePagingQuery> MapToDomainRequest(EmployeePagingApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return (apiRequest.Id ?? [])
      .ToSeq()
      .Map(_dataIdentityToDomainIdentityMapper.Map)
      .Match<Either<EmployeeListingQuery, EmployeePagingQuery>>(
        () => new EmployeePagingQuery(
          tenantId,
          userId,
          apiRequest.Search ?? string.Empty,
          new Page(
            apiRequest.PageNumber ?? 0,
            apiRequest.PageSize ?? 0)),
        ids => new EmployeeListingQuery(
          ids,
          tenantId,
          userId));
  }
}
