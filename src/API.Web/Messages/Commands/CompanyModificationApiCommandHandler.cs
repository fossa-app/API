using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanyModificationApiCommandHandler : ApiMessageHandler<CompanyId, CompanyModificationApiCommand, Unit, CompanyModificationCommand, Unit>
{
  private readonly IRegionFactory _regionFactory;

  public CompanyModificationApiCommandHandler(
    IRegionFactory regionFactory,
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper) : base(
      sender,
      tenantIdProvider,
      domainIdentityToDataIdentityMapper,
      dataIdentityToDomainIdentityMapper)
  {
    _regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override CompanyModificationCommand MapToDomainRequest(CompanyModificationApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    return new CompanyModificationCommand(
      tenantId,
      apiRequest.Name ?? string.Empty,
      _regionFactory.Create(apiRequest.CountryCode ?? string.Empty));
  }
}
