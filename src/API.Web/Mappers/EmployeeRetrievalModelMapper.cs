using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class EmployeeRetrievalModelMapper : IMapper<EmployeeEntity, EmployeeRetrievalModel>
{
  private readonly IMapper<BranchId, long> _branchDomainToDataIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainToDataIdentityMapper;
  private readonly IMapper<DepartmentId, long> _departmentDomainToDataIdentityMapper;
  private readonly IMapper<EmployeeId, long> _employeeDomainToDataIdentityMapper;

  public EmployeeRetrievalModelMapper(
      IMapper<EmployeeId, long> employeeDomainToDataIdentityMapper,
      IMapper<CompanyId, long> companyDomainToDataIdentityMapper,
      IMapper<BranchId, long> branchDomainToDataIdentityMapper,
      IMapper<DepartmentId, long> departmentDomainToDataIdentityMapper)
  {
    _employeeDomainToDataIdentityMapper = employeeDomainToDataIdentityMapper ?? throw new ArgumentNullException(nameof(employeeDomainToDataIdentityMapper));
    _companyDomainToDataIdentityMapper = companyDomainToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainToDataIdentityMapper));
    _branchDomainToDataIdentityMapper = branchDomainToDataIdentityMapper ?? throw new ArgumentNullException(nameof(branchDomainToDataIdentityMapper));
    _departmentDomainToDataIdentityMapper = departmentDomainToDataIdentityMapper ?? throw new ArgumentNullException(nameof(departmentDomainToDataIdentityMapper));
  }

  public EmployeeRetrievalModel Map(EmployeeEntity source)
  {
    return new EmployeeRetrievalModel(
        _employeeDomainToDataIdentityMapper.Map(source.ID),
        _companyDomainToDataIdentityMapper.Map(source.CompanyId),
        source.AssignedBranchId.Map(_branchDomainToDataIdentityMapper.Map).MatchUnsafe(s => s, (long?)null),
        source.AssignedDepartmentId.Map(_departmentDomainToDataIdentityMapper.Map).MatchUnsafe(x => x, () => (long?)null),
        source.ReportsToId.Map(_employeeDomainToDataIdentityMapper.Map).MatchUnsafe(x => x, () => (long?)null),
        source.JobTitle,
        source.FirstName, source.LastName, source.FullName);
  }
}
