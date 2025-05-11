using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class DepartmentDeletionApiCommandHandler : ApiMessageHandler<DepartmentId, DepartmentDeletionApiCommand, Unit, DepartmentDeletionCommand, Unit>
{
  public DepartmentDeletionApiCommandHandler(
      ISender sender,
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, DepartmentId> dataIdentityToDomainIdentityMapper)
      : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override DepartmentDeletionCommand MapToDomainRequest(DepartmentDeletionApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return new DepartmentDeletionCommand(
        _dataIdentityToDomainIdentityMapper.Map(apiRequest.Id),
        tenantId,
        userId);
  }
}
