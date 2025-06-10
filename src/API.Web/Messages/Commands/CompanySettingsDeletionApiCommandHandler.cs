using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanySettingsDeletionApiCommandHandler :
    ApiMessageHandler<CompanySettingsId, CompanySettingsDeletionApiCommand, Unit, CompanySettingsDeletionCommand, Unit>
{
  public CompanySettingsDeletionApiCommandHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<CompanySettingsId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanySettingsId> dataIdentityToDomainIdentityMapper)
    : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
  }

  protected override Unit MapToApiResponse(Unit domainResponse)
  {
    return domainResponse;
  }

  protected override CompanySettingsDeletionCommand MapToDomainRequest(CompanySettingsDeletionApiCommand apiRequest)
  {
    return new CompanySettingsDeletionCommand(
      _tenantIdProvider.GetTenantId(),
      _userIdProvider.GetUserId());
  }
}
