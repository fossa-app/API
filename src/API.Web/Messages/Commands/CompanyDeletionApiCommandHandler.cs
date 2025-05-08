using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanyDeletionApiCommandHandler : ApiMessageHandler<CompanyId, CompanyDeletionApiCommand, Unit, CompanyDeletionCommand, Unit>
{
  public CompanyDeletionApiCommandHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper) : base(
      sender,
      tenantIdProvider,
      domainIdentityToDataIdentityMapper,
      dataIdentityToDomainIdentityMapper)
  {
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override CompanyDeletionCommand MapToDomainRequest(CompanyDeletionApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    return new CompanyDeletionCommand(
      tenantId);
  }
}
