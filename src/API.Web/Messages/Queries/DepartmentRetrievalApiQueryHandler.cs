using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class DepartmentRetrievalApiQueryHandler :
    ApiMessageHandler<DepartmentId, DepartmentRetrievalApiQuery, DepartmentRetrievalModel, DepartmentRetrievalQuery, DepartmentEntity>
{
  private readonly IMapper<DepartmentEntity, DepartmentRetrievalModel> _domainResponseToApiResponseMapper;

  public DepartmentRetrievalApiQueryHandler(
      IMapper<DepartmentEntity, DepartmentRetrievalModel> domainResponseToApiResponseMapper,
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
    _domainResponseToApiResponseMapper = domainResponseToApiResponseMapper ?? throw new ArgumentNullException(nameof(domainResponseToApiResponseMapper));
  }

  protected override DepartmentRetrievalModel MapToApiResponse(DepartmentEntity domainResponse)
  {
    return _domainResponseToApiResponseMapper.Map(domainResponse);
  }

  protected override DepartmentRetrievalQuery MapToDomainRequest(DepartmentRetrievalApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new DepartmentRetrievalQuery(
        _dataIdentityToDomainIdentityMapper.Map(apiRequest.Id),
        tenantId,
        userId);
  }
}
