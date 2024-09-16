using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class BranchMongoMapper : IMapper<BranchMongoEntity, BranchEntity>,
  IMapper<BranchEntity, BranchMongoEntity>
{
  private readonly IMapper<BranchId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<long, BranchId> _dataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;

  public BranchMongoMapper(
    IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, BranchId> dataIdentityToDomainIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _companyDataIdentityToDomainIdentityMapper = companyDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(companyDataIdentityToDomainIdentityMapper));
  }

  public BranchEntity Map(BranchMongoEntity source)
  {
    return new BranchEntity(
      _dataIdentityToDomainIdentityMapper.Map(source.ID),
      source.TenantID,
      source.UserID,
      _companyDataIdentityToDomainIdentityMapper.Map(source.CompanyId),
      source.Name ?? string.Empty);
  }

  public BranchMongoEntity Map(BranchEntity source)
  {
    return new BranchMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      TenantID = source.TenantID,
      UserID = source.UserID,
      CompanyId = _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      Name = source.Name,
    };
  }
}
