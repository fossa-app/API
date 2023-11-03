using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class EmployeeRetrievalModelMapper : IMapper<EmployeeEntity, EmployeeRetrievalModel>
{
  private readonly IMapper<EmployeeId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<long, EmployeeId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;

  public EmployeeRetrievalModelMapper(
    IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ??
                                          throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ??
                                          throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ??
                                                 throw new ArgumentNullException(
                                                   nameof(companyDomainIdentityToDataIdentityMapper));
    _companyDataIdentityToDomainIdentityMapper = companyDataIdentityToDomainIdentityMapper ??
                                                 throw new ArgumentNullException(
                                                   nameof(companyDataIdentityToDomainIdentityMapper));
  }

  public EmployeeRetrievalModel Map(EmployeeEntity source)
  {
    return new EmployeeRetrievalModel(
      _domainIdentityToDataIdentityMapper.Map(source.ID),
      _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      source.FirstName, source.LastName, source.FullName);
  }
}
