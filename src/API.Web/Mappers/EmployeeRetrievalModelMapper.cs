using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class EmployeeRetrievalModelMapper : IMapper<EmployeeEntity, EmployeeRetrievalModel>
{
  private readonly IMapper<EmployeeId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;

  public EmployeeRetrievalModelMapper(
    IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
  }

  public EmployeeRetrievalModel Map(EmployeeEntity source)
  {
    return new EmployeeRetrievalModel(
      _domainIdentityToDataIdentityMapper.Map(source.ID),
      _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      source.FirstName, source.LastName, source.FullName);
  }
}
