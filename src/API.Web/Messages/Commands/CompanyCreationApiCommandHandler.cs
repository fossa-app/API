using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using TIKSN.Globalization;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class CompanyCreationApiCommandHandler : ApiMessageHandler<CompanyId, CompanyCreationApiCommand, Unit, CompanyCreationCommand, Unit>
{
  private readonly IRegionFactory _regionFactory;

  public CompanyCreationApiCommandHandler(
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

  protected override CompanyCreationCommand MapToDomainRequest(CompanyCreationApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    return new CompanyCreationCommand(
      tenantId,
      apiRequest.Name ?? string.Empty,
      _regionFactory.Create(apiRequest.CountryCode ?? string.Empty));
  }
}
