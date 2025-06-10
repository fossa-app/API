using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanySettingsCreationApiCommandHandler :
    ApiMessageHandler<CompanySettingsId, CompanySettingsCreationApiCommand, Unit, CompanySettingsCreationCommand, Unit>
{
  private readonly IMapper<string, ColorSchemeId> _colorSchemeDataToDomainMapper;

  public CompanySettingsCreationApiCommandHandler(
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

  protected override CompanySettingsCreationCommand MapToDomainRequest(CompanySettingsCreationApiCommand apiRequest)
  {
    return new CompanySettingsCreationCommand(
      _tenantIdProvider.GetTenantId(),
      _userIdProvider.GetUserId(),
      _colorSchemeDataToDomainMapper.Map(apiRequest.ColorSchemeId ?? string.Empty));
  }
}
