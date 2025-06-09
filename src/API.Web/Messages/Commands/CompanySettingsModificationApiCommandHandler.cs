using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanySettingsModificationApiCommandHandler :
    ApiMessageHandler<CompanySettingsId, CompanySettingsModificationApiCommand, Unit, CompanySettingsModificationCommand, Unit>
{
  private readonly IMapper<string, ColorSchemeId> _colorSchemeDataToDomainMapper;

  public CompanySettingsModificationApiCommandHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<CompanySettingsId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanySettingsId> dataIdentityToDomainIdentityMapper,
    IMapper<string, ColorSchemeId> colorSchemeDataToDomainMapper)
    : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
    _colorSchemeDataToDomainMapper = colorSchemeDataToDomainMapper ?? throw new ArgumentNullException(nameof(colorSchemeDataToDomainMapper));
  }

  protected override Unit MapToApiResponse(Unit domainResponse)
  {
    return domainResponse;
  }

  protected override CompanySettingsModificationCommand MapToDomainRequest(CompanySettingsModificationApiCommand apiRequest)
  {
    return new CompanySettingsModificationCommand(
      _dataIdentityToDomainIdentityMapper.Map(apiRequest.ID),
      _tenantIdProvider.GetTenantId(),
      _userIdProvider.GetUserId(),
      _colorSchemeDataToDomainMapper.Map(apiRequest.ColorSchemeId ?? string.Empty));
  }
}
