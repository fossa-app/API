using System.Globalization;
using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class CompanyRetrievalModelMapper : IMapper<CompanyEntity, CompanyRetrievalModel>
{
  private readonly IMapper<CompanyId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<RegionInfo, CountryModel> _regionInfoToCountryModelMapper;

  public CompanyRetrievalModelMapper(
    IMapper<CompanyId, long> domainIdentityToDataIdentityMapper,
    IMapper<RegionInfo, CountryModel> regionInfoToCountryModelMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _regionInfoToCountryModelMapper = regionInfoToCountryModelMapper ?? throw new ArgumentNullException(nameof(regionInfoToCountryModelMapper));
  }

  public CompanyRetrievalModel Map(CompanyEntity source)
  {
    return new CompanyRetrievalModel(
      _domainIdentityToDataIdentityMapper.Map(source.ID),
      source.Name,
      _regionInfoToCountryModelMapper.Map(source.Country));
  }
}
