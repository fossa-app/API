using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class EmployeeRetrievalApiQueryHandler :
    ApiMessageHandler<EmployeeId, EmployeeRetrievalApiQuery, EmployeeRetrievalModel, EmployeeRetrievalQuery, EmployeeEntity>
{
  private readonly IMapper<EmployeeEntity, EmployeeRetrievalModel> _domainResponseToApiResponseMapper;

  public EmployeeRetrievalApiQueryHandler(
      IMapper<EmployeeEntity, EmployeeRetrievalModel> domainResponseToApiResponseMapper,
      ISender sender,
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper)
      : base(
          sender,
          tenantIdProvider,
          userIdProvider,
          domainIdentityToDataIdentityMapper,
          dataIdentityToDomainIdentityMapper)
  {
    _domainResponseToApiResponseMapper = domainResponseToApiResponseMapper ?? throw new ArgumentNullException(nameof(domainResponseToApiResponseMapper));
  }

  protected override EmployeeRetrievalModel MapToApiResponse(EmployeeEntity domainResponse)
  {
    return _domainResponseToApiResponseMapper.Map(domainResponse);
  }

  protected override EmployeeRetrievalQuery MapToDomainRequest(EmployeeRetrievalApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return new EmployeeRetrievalQuery(
        _dataIdentityToDomainIdentityMapper.Map(apiRequest.Id),
        tenantId,
        userId);
  }
}
