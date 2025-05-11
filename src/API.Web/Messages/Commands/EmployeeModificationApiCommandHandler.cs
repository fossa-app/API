using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class EmployeeModificationApiCommandHandler : ApiMessageHandler<EmployeeId, EmployeeModificationApiCommand, Unit, EmployeeModificationCommand, Unit>
{
  public EmployeeModificationApiCommandHandler(
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

  protected override EmployeeModificationCommand MapToDomainRequest(EmployeeModificationApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new EmployeeModificationCommand(
        tenantId,
        userId,
        apiRequest.FirstName ?? string.Empty,
        apiRequest.LastName ?? string.Empty,
        apiRequest.FullName ?? string.Empty);
  }
}
