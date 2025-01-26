using Fossa.API.Core.Entities;
using Fossa.API.Core.TimeZone;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class BranchMongoMapper :
  IMapper<BranchMongoEntity, BranchEntity>,
  IMapper<BranchEntity, BranchMongoEntity>
{
  private readonly IMapper<AddressMongo, Address> _addressDataToDomainMapper;
  private readonly IMapper<Address, AddressMongo> _addressDomainToDataMapper;
  private readonly IMapper<long, CompanyId> _companyDataIdentityToDomainIdentityMapper;
  private readonly IMapper<CompanyId, long> _companyDomainIdentityToDataIdentityMapper;
  private readonly IMapper<long, BranchId> _dataIdentityToDomainIdentityMapper;
  private readonly IDateTimeZoneProvider _dateTimeZoneProvider;
  private readonly IMapper<BranchId, long> _domainIdentityToDataIdentityMapper;

  public BranchMongoMapper(
    IMapper<BranchId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, BranchId> dataIdentityToDomainIdentityMapper,
    IMapper<CompanyId, long> companyDomainIdentityToDataIdentityMapper,
    IMapper<long, CompanyId> companyDataIdentityToDomainIdentityMapper,
    IMapper<Address, AddressMongo> addressDomainToDataMapper,
    IMapper<AddressMongo, Address> addressDataToDomainMapper,
    IDateTimeZoneProvider dateTimeZoneProvider)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
    _companyDomainIdentityToDataIdentityMapper = companyDomainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(companyDomainIdentityToDataIdentityMapper));
    _companyDataIdentityToDomainIdentityMapper = companyDataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(companyDataIdentityToDomainIdentityMapper));
    _addressDomainToDataMapper = addressDomainToDataMapper ?? throw new ArgumentNullException(nameof(addressDomainToDataMapper));
    _addressDataToDomainMapper = addressDataToDomainMapper ?? throw new ArgumentNullException(nameof(addressDataToDomainMapper));
    _dateTimeZoneProvider = dateTimeZoneProvider ?? throw new ArgumentNullException(nameof(dateTimeZoneProvider));
  }

  public BranchEntity Map(BranchMongoEntity source)
  {
    return new BranchEntity(
      _dataIdentityToDomainIdentityMapper.Map(source.ID),
      source.TenantID,
      _companyDataIdentityToDomainIdentityMapper.Map(source.CompanyId),
      source.Name ?? string.Empty,
      _dateTimeZoneProvider.GetDateTimeZoneById(source.TimeZoneId ?? string.Empty),
      Optional(source.Address).Map(_addressDataToDomainMapper.Map));
  }

  public BranchMongoEntity Map(BranchEntity source)
  {
    return new BranchMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      TenantID = source.TenantID,
      CompanyId = _companyDomainIdentityToDataIdentityMapper.Map(source.CompanyId),
      Name = source.Name,
      TimeZoneId = source.TimeZone.Id,
      Address = source.Address.Map(_addressDomainToDataMapper.Map).MatchUnsafe(x => x, () => null),
    };
  }
}
