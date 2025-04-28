using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class DepartmentMongoMapper :
    IMapper<DepartmentMongoEntity, DepartmentEntity>,
    IMapper<DepartmentEntity, DepartmentMongoEntity>
{
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, DepartmentId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<DepartmentId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<long, EmployeeId> _employeeDataIdentityToDomainIdentityMapper;
  private readonly IMapper<EmployeeId, long> _employeeDomainIdentityToDataIdentityMapper;

  public DepartmentMongoMapper(
      IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, DepartmentId> dataIdentityToDomainIdentityMapper,
      IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
      IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper,
      IMapper<EmployeeId, long> employeeDomainIdentityToDataIdentityMapper,
      IMapper<long, EmployeeId> employeeDataIdentityToDomainIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _companyDataIdentityToDomainIdentityMapper = companyDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(companyDataIdentityToDomainIdentityMapper));
    _employeeDomainIdentityToDataIdentityMapper = employeeDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(employeeDomainIdentityToDataIdentityMapper));
    _employeeDataIdentityToDomainIdentityMapper = employeeDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(employeeDataIdentityToDomainIdentityMapper));
  }

  public DepartmentEntity Map(DepartmentMongoEntity source)
  {
    return new DepartmentEntity(
        _dataIdentityToDomainIdentityMapper.Map(source.ID),
        source.TenantID,
        _companyDataIdentityToDomainIdentityMapper.Map(source.CompanyId),
        source.Name ?? string.Empty,
        _employeeDataIdentityToDomainIdentityMapper.Map(source.ManagerId),
        Optional(source.ParentDepartmentId).Map(_dataIdentityToDomainIdentityMapper.Map));
  }

  public DepartmentMongoEntity Map(DepartmentEntity source)
  {
    return new DepartmentMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      TenantID = source.TenantID,
      CompanyId = _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      Name = source.Name,
      ParentDepartmentId = source.ParentDepartmentId.Map(_domainIdentityToDataIdentityMapper.Map).MatchUnsafe(x => x, () => (long?)null),
      ManagerId = _employeeDomainIdentityToDataIdentityMapper.Map(source.ManagerId)
    };
  }
}
