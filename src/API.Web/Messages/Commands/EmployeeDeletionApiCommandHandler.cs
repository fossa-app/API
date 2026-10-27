using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using TIKSN.Identity;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class EmployeeDeletionApiCommandHandler : ApiMessageHandler<EmployeeId, EmployeeDeletionApiCommand, Unit, EmployeeDeletionCommand, Unit>
{
  public EmployeeDeletionApiCommandHandler(
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
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override EmployeeDeletionCommand MapToDomainRequest(EmployeeDeletionApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new EmployeeDeletionCommand(
        tenantId,
        userId);
  }
}
