using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class SystemPropertiesMongoMapper
  : IMapper<SystemPropertiesMongoEntity, SystemPropertiesEntity>
    , IMapper<SystemPropertiesEntity, SystemPropertiesMongoEntity>
{
  private readonly IMapper<SystemPropertiesId, long> _domainIdentityToDataIdentityMapper;
  private readonly IMapper<long, SystemPropertiesId> _dataIdentityToDomainIdentityMapper;

  public SystemPropertiesMongoMapper(
    IMapper<SystemPropertiesId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, SystemPropertiesId> dataIdentityToDomainIdentityMapper)
  {
    _domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ??
                                          throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
    _dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ??
                                          throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
  }

  public SystemPropertiesEntity Map(SystemPropertiesMongoEntity source)
  {
    return new SystemPropertiesEntity(_dataIdentityToDomainIdentityMapper.Map(source.ID), new Ulid(source.SystemID));
  }

  public SystemPropertiesMongoEntity Map(SystemPropertiesEntity source)
  {
    return new SystemPropertiesMongoEntity
    {
      ID = _domainIdentityToDataIdentityMapper.Map(source.ID),
      SystemID = source.SystemID.ToByteArray(),
    };
  }
}
