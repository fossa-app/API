using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class DepartmentRetrievalModelMapper : IMapper<DepartmentEntity, DepartmentRetrievalModel>
{
  private readonly IMapper<DepartmentId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<EmployeeId, long> _employeeDomainIdentityToDataIdentityMapper;

  public DepartmentRetrievalModelMapper(
      IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper,
      IMapper<EmployeeId, long> employeeDomainIdentityToDataIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _employeeDomainIdentityToDataIdentityMapper = employeeDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(employeeDomainIdentityToDataIdentityMapper));
  }

  public DepartmentRetrievalModel Map(DepartmentEntity source)
  {
    return new DepartmentRetrievalModel(
        _domainIdentityToDataIdentityMapper.Map(source.ID),
        source.Name,
        source.ParentDepartmentId.Map(_domainIdentityToDataIdentityMapper.Map).MatchUnsafe(x => x, () => (long?)null),
        _employeeDomainIdentityToDataIdentityMapper.Map(source.ManagerId));
  }
}
