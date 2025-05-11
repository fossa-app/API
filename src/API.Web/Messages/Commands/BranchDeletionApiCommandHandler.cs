using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class BranchDeletionApiCommandHandler
    : ApiMessageHandler<BranchId, BranchDeletionApiCommand, Unit, BranchDeletionCommand, Unit>
{
  public BranchDeletionApiCommandHandler(
      ISender sender,
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, BranchId> dataIdentityToDomainIdentityMapper)
      : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
  }

  protected override Unit MapToApiResponse(Unit domainResponse)
  {
    return domainResponse;
  }

  protected override BranchDeletionCommand MapToDomainRequest(BranchDeletionApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();

    return new BranchDeletionCommand(
        _dataIdentityToDomainIdentityMapper.Map(apiRequest.Id),
        tenantId,
        userId);
  }
}
