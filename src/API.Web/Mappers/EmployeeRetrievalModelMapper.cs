using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class EmployeeRetrievalModelMapper : IMapper<EmployeeEntity, EmployeeRetrievalModel>
{
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;

  public EmployeeRetrievalModelMapper(
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper)
  {
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
      source.ID, _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      source.FirstName, source.LastName, source.FullName);
  }
}
