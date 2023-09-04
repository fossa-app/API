using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class SystemPropertiesMongoMapper
  : IMapper<SystemPropertiesMongoEntity, SystemPropertiesEntity>
    , IMapper<SystemPropertiesEntity, SystemPropertiesMongoEntity>
{
  public SystemPropertiesEntity Map(SystemPropertiesMongoEntity source)
  {
    return new SystemPropertiesEntity(source.ID, new Ulid(source.SystemID));
  }

  public SystemPropertiesMongoEntity Map(SystemPropertiesEntity source)
  {
    return new SystemPropertiesMongoEntity { ID = source.ID, SystemID = source.SystemID.ToByteArray(), };
  }
}
