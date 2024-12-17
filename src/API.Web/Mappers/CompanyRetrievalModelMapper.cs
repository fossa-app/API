using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class CompanyRetrievalModelMapper : IMapper<CompanyEntity, CompanyRetrievalModel>
{
  private readonly IMapper<CompanyId, long> _domainIdentityToDataIdentityMapper;

  public CompanyRetrievalModelMapper(
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
  }

  public CompanyRetrievalModel Map(CompanyEntity source)
  {
    return new CompanyRetrievalModel(
      _domainIdentityToDataIdentityMapper.Map(source.ID),
      source.Name,
      source.Country.TwoLetterISORegionName);
  }
}
