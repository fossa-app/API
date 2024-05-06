using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class EmployeeMongoMapper : IMapper<EmployeeMongoEntity, EmployeeEntity>,
  IMapper<EmployeeEntity, EmployeeMongoEntity>
{
  private readonly IMapper<EmployeeId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<long, EmployeeId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;

  public EmployeeMongoMapper(
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

  public EmployeeEntity Map(EmployeeMongoEntity source)
  {
    return new EmployeeEntity(
      _dataIdentityToDomainIdentityMapper.Map(source.ID),
      source.TenantID,
      source.UserID,
      _companyDataIdentityToDomainIdentityMapper.Map(source.CompanyId),
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
      FirstName = source.FirstName,
      LastName = source.LastName,
      FullName = source.FullName,
    };
  }
}
