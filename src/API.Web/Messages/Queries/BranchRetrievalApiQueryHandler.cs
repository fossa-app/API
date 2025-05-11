using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class BranchRetrievalApiQueryHandler :
    ApiMessageHandler<BranchId, BranchRetrievalApiQuery, BranchRetrievalModel, BranchRetrievalQuery, BranchEntity>
{
  private readonly IMapper<BranchEntity, BranchRetrievalModel> _domainResponseToApiResponseMapper;

  public BranchRetrievalApiQueryHandler(
      IMapper<BranchEntity, BranchRetrievalModel> domainResponseToApiResponseMapper,
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
    _domainResponseToApiResponseMapper = domainResponseToApiResponseMapper ?? throw new ArgumentNullException(nameof(domainResponseToApiResponseMapper));
  }

  protected override BranchRetrievalQuery MapToDomainRequest(BranchRetrievalApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new BranchRetrievalQuery(
        _dataIdentityToDomainIdentityMapper.Map(apiRequest.Id),
        tenantId,
        userId);
  }

  protected override BranchRetrievalModel MapToApiResponse(BranchEntity domainResponse)
  {
    return _domainResponseToApiResponseMapper.Map(domainResponse);
  }
}
