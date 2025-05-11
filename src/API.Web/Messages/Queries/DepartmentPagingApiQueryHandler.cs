using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Data;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class DepartmentPagingApiQueryHandler :
    ApiMessageHandler<DepartmentId, DepartmentPagingApiQuery, PagingResponseModel<DepartmentRetrievalModel>, DepartmentListingQuery, Seq<DepartmentEntity>, DepartmentPagingQuery, PageResult<DepartmentEntity>>
{
  private readonly IMapper<Seq<DepartmentEntity>, PagingResponseModel<DepartmentRetrievalModel>> _listingMapper;
  private readonly IMapper<PageResult<DepartmentEntity>, PagingResponseModel<DepartmentRetrievalModel>> _pagingMapper;

  public DepartmentPagingApiQueryHandler(
      IMapper<PageResult<DepartmentEntity>, PagingResponseModel<DepartmentRetrievalModel>> pagingMapper,
      IMapper<Seq<DepartmentEntity>, PagingResponseModel<DepartmentRetrievalModel>> listingMapper,
      ISender sender,
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, DepartmentId> dataIdentityToDomainIdentityMapper) : base(
          sender,
          tenantIdProvider,
          userIdProvider,
          domainIdentityToDataIdentityMapper,
          dataIdentityToDomainIdentityMapper)
  {
    _pagingMapper = pagingMapper ?? throw new ArgumentNullException(nameof(pagingMapper));
    _listingMapper = listingMapper ?? throw new ArgumentNullException(nameof(listingMapper));
  }

  protected override PagingResponseModel<DepartmentRetrievalModel> MapToApiResponse(PageResult<DepartmentEntity> domainResponse)
  {
    return _pagingMapper.Map(domainResponse);
  }

  protected override PagingResponseModel<DepartmentRetrievalModel> MapToApiResponse(Seq<DepartmentEntity> domainResponse)
  {
    return _listingMapper.Map(domainResponse);
  }

  protected override Either<DepartmentListingQuery, DepartmentPagingQuery> MapToDomainRequest(DepartmentPagingApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return (apiRequest.Id ?? [])
        .ToSeq()
        .Map(_dataIdentityToDomainIdentityMapper.Map)
        .Match<Either<DepartmentListingQuery, DepartmentPagingQuery>>(
            () => new DepartmentPagingQuery(
                tenantId,
                userId,
                apiRequest.Search ?? string.Empty,
                new Page(
                    apiRequest.PageNumber ?? 0,
                    apiRequest.PageSize ?? 0)),
            ids => new DepartmentListingQuery(
                ids,
                tenantId,
                userId));
  }
}
