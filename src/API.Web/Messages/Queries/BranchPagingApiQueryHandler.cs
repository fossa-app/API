using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Data;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class BranchPagingApiQueryHandler :
    ApiMessageHandler<BranchId, BranchPagingApiQuery, PagingResponseModel<BranchRetrievalModel>, BranchListingQuery, Seq<BranchEntity>, BranchPagingQuery, PageResult<BranchEntity>>
{
    private readonly IMapper<Seq<BranchEntity>, PagingResponseModel<BranchRetrievalModel>> _listingMapper;
    private readonly IMapper<PageResult<BranchEntity>, PagingResponseModel<BranchRetrievalModel>> _pagingMapper;

    public BranchPagingApiQueryHandler(
        IMapper<PageResult<BranchEntity>, PagingResponseModel<BranchRetrievalModel>> pagingMapper,
        IMapper<Seq<BranchEntity>, PagingResponseModel<BranchRetrievalModel>> listingMapper,
        ISender sender,
        ITenantIdProvider<Guid> tenantIdProvider,
        IUserIdProvider<Guid> userIdProvider,
        IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
        IMapper<long, BranchId> dataIdentityToDomainIdentityMapper) : base(
            sender,
            tenantIdProvider,
            userIdProvider,
            domainIdentityToDataIdentityMapper,
            dataIdentityToDomainIdentityMapper)
    {
        _pagingMapper = pagingMapper ?? throw new ArgumentNullException(nameof(pagingMapper));
        _listingMapper = listingMapper ?? throw new ArgumentNullException(nameof(listingMapper));
    }

    protected override PagingResponseModel<BranchRetrievalModel> MapToApiResponse(PageResult<BranchEntity> domainResponse)
    {
        return _pagingMapper.Map(domainResponse);
    }

    protected override PagingResponseModel<BranchRetrievalModel> MapToApiResponse(Seq<BranchEntity> domainResponse)
    {
        return _listingMapper.Map(domainResponse);
    }

    protected override Either<BranchListingQuery, BranchPagingQuery> MapToDomainRequest(BranchPagingApiQuery apiRequest)
    {
        var tenantId = _tenantIdProvider.GetTenantId();
        var userId = _userIdProvider.GetUserId();

        return (apiRequest.Id ?? [])
            .ToSeq()
            .Map(_dataIdentityToDomainIdentityMapper.Map)
            .Match<Either<BranchListingQuery, BranchPagingQuery>>(
                () => new BranchPagingQuery(
                    tenantId,
                    userId,
                    apiRequest.Search ?? string.Empty,
                    new Page(
                        apiRequest.PageNumber ?? 0,
                        apiRequest.PageSize ?? 0)),
                ids => new BranchListingQuery(
                    ids,
                    tenantId,
                    userId));
    }
}
