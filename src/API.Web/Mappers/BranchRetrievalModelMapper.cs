using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class BranchRetrievalModelMapper : IMapper<BranchEntity, BranchRetrievalModel>
{
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<BranchId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<Address, AddressModel> _domainToModelMapper;

  public BranchRetrievalModelMapper(
    IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<Address, AddressModel> domainToModelMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _domainToModelMapper = domainToModelMapper ?? throw new ArgumentNullException(nameof(domainToModelMapper));
  }

  public BranchRetrievalModel Map(BranchEntity source)
  {
    return new BranchRetrievalModel(
      _domainIdentityToDataIdentityMapper.Map(source.ID),
      _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      source.Name,
      source.TimeZone.Id,
      source.Address.Map(_domainToModelMapper.Map).MatchUnsafe(x => x, () => null));
  }
}
