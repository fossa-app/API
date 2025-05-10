using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class CurrentEmployeeRetrievalApiQueryHandler : ApiMessageHandler<EmployeeId, CurrentEmployeeRetrievalApiQuery, EmployeeRetrievalModel, CurrentEmployeeRetrievalQuery, EmployeeEntity>
{
  private readonly IMapper<EmployeeEntity, EmployeeRetrievalModel> _domainResponseToApiResponseMapper;

  public CurrentEmployeeRetrievalApiQueryHandler(
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

  protected override CurrentEmployeeRetrievalQuery MapToDomainRequest(CurrentEmployeeRetrievalApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new CurrentEmployeeRetrievalQuery(tenantId, userId);
  }
}
