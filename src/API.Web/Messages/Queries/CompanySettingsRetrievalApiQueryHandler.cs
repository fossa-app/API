using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class CompanySettingsRetrievalApiQueryHandler :
    ApiMessageHandler<CompanySettingsId, CompanySettingsRetrievalApiQuery, CompanySettingsRetrievalModel, CompanySettingsRetrievalQuery, CompanySettingsEntity>
{
  private readonly IMapper<CompanySettingsEntity, CompanySettingsRetrievalModel> _domainResponseToApiResponseMapper;

  public CompanySettingsRetrievalApiQueryHandler(
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IUserIdProvider<Guid> userIdProvider,
    IMapper<CompanySettingsId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanySettingsId> dataIdentityToDomainIdentityMapper,
    IMapper<CompanySettingsEntity, CompanySettingsRetrievalModel> domainResponseToApiResponseMapper)
    : base(sender, tenantIdProvider, userIdProvider, domainIdentityToDataIdentityMapper, dataIdentityToDomainIdentityMapper)
  {
    _domainResponseToApiResponseMapper = domainResponseToApiResponseMapper ?? throw new ArgumentNullException(nameof(domainResponseToApiResponseMapper));
  }

  protected override CompanySettingsRetrievalModel MapToApiResponse(CompanySettingsEntity domainResponse)
  {
    return _domainResponseToApiResponseMapper.Map(domainResponse);
  }

  protected override CompanySettingsRetrievalQuery MapToDomainRequest(CompanySettingsRetrievalApiQuery apiRequest)
  {
    return new CompanySettingsRetrievalQuery(_tenantIdProvider.GetTenantId());
  }
}
