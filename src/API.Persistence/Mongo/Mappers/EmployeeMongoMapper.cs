using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class EmployeeMongoMapper : IMapper<EmployeeMongoEntity, EmployeeEntity>,
  IMapper<EmployeeEntity, EmployeeMongoEntity>
{
  private readonly IMapper<long, BranchId> _branchDataIdentityToDomainIdentityMapper;
  private readonly IMapper<BranchId, long> _branchDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, EmployeeId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<long, DepartmentId> _departmentDataToDomainIdentityMapper;
  private readonly IMapper<DepartmentId, long> _departmentDomainToDataIdentityMapper;
  private readonly IMapper<EmployeeId, long> _domainIdentityToDataIdentityMapper;

  public EmployeeMongoMapper(
    IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper,
    IMapper<DepartmentId, long> departmentDomainToDataIdentityMapper,
    IMapper<long, DepartmentId> departmentDataToDomainIdentityMapper,
    IMapper<BranchId, long> branchDomainIdentityToDataIdentityMapper,
    IMapper<long, BranchId> branchDataIdentityToDomainIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _companyDataIdentityToDomainIdentityMapper = companyDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(companyDataIdentityToDomainIdentityMapper));
    _branchDomainIdentityToDataIdentityMapper = branchDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(branchDomainIdentityToDataIdentityMapper));
    _departmentDomainToDataIdentityMapper = departmentDomainToDataIdentityMapper ?? throw new ArgumentNullException(nameof(departmentDomainToDataIdentityMapper));
    _departmentDataToDomainIdentityMapper = departmentDataToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(departmentDataToDomainIdentityMapper));
    _branchDataIdentityToDomainIdentityMapper = branchDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(branchDataIdentityToDomainIdentityMapper));
  }

  public EmployeeEntity Map(EmployeeMongoEntity source)
  {
    return new EmployeeEntity(
      _dataIdentityToDomainIdentityMapper.Map(source.ID),
      source.TenantID,
      source.UserID,
      _companyDataIdentityToDomainIdentityMapper.Map(source.CompanyId),
      Optional(source.AssignedBranchId).Map(_branchDataIdentityToDomainIdentityMapper.Map),
      Optional(source.AssignedDepartmentId).Map(_departmentDataToDomainIdentityMapper.Map),
      Optional(source.ReportsToId).Map(_dataIdentityToDomainIdentityMapper.Map),
      source.FirstName ?? string.Empty,
      source.LastName ?? string.Empty,
      source.FullName ?? string.Empty);
  }

  public EmployeeMongoEntity Map(EmployeeEntity source)
  {
    return new EmployeeMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      TenantID = source.TenantID,
      UserID = source.UserID,
      CompanyId = _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      AssignedBranchId = source.AssignedBranchId.Map(_branchDomainIdentityToDataIdentityMapper.Map).MatchUnsafe(s => s, (long?)null),
      AssignedDepartmentId = source.AssignedDepartmentId.Map(_departmentDomainToDataIdentityMapper.Map).MatchUnsafe(s => s, (long?)null),
      ReportsToId = source.ReportsToId.Map(_domainIdentityToDataIdentityMapper.Map).MatchUnsafe(s => s, (long?)null),
      FirstName = source.FirstName,
      LastName = source.LastName,
      FullName = source.FullName,
    };
  }
}
