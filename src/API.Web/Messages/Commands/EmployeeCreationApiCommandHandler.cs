using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class EmployeeCreationApiCommandHandler : ApiMessageHandler<EmployeeId, EmployeeCreationApiCommand, Unit, EmployeeCreationCommand, Unit>
{
  public EmployeeCreationApiCommandHandler(
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

  protected override EmployeeCreationCommand MapToDomainRequest(EmployeeCreationApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new EmployeeCreationCommand(
        tenantId,
        userId,
        apiRequest.FirstName ?? string.Empty,
        apiRequest.LastName ?? string.Empty,
        apiRequest.FullName ?? string.Empty);
  }
}
