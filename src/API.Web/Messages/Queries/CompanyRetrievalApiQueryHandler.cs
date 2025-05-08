using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Queries;
using Fossa.API.Core.Tenant;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Queries;

public class CompanyRetrievalApiQueryHandler : ApiMessageHandler<CompanyId, CompanyRetrievalApiQuery, CompanyRetrievalModel, CompanyRetrievalQuery, CompanyEntity>
{
  private readonly IMapper<CompanyEntity, CompanyRetrievalModel> _domainResponseToApiResponseMapper;

  public CompanyRetrievalApiQueryHandler(
    IMapper<CompanyEntity, CompanyRetrievalModel> domainResponseToApiResponseMapper,
    ISender sender,
    ITenantIdProvider<Guid> tenantIdProvider,
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> dataIdentityToDomainIdentityMapper) : base(
      sender,
      tenantIdProvider,
      domainIdentityToDataIdentityMapper,
      dataIdentityToDomainIdentityMapper)
  {
    _domainResponseToApiResponseMapper = domainResponseToApiResponseMapper ?? throw new ArgumentNullException(nameof(domainResponseToApiResponseMapper));
  }

  protected override CompanyRetrievalModel MapToApiResponse(CompanyEntity domainResponse)
  {
    return _domainResponseToApiResponseMapper.Map(domainResponse);
  }

  protected override CompanyRetrievalQuery MapToDomainRequest(CompanyRetrievalApiQuery apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    return new CompanyRetrievalQuery(tenantId);
  }
}
