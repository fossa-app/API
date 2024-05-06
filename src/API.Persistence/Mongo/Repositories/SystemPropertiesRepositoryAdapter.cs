using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class SystemPropertiesRepositoryAdapter
  : MongoRepositoryAdapter<SystemPropertiesEntity, SystemPropertiesId, SystemPropertiesMongoEntity, long>
    , ISystemPropertiesRepository, ISystemPropertiesQueryRepository
{
  public SystemPropertiesRepositoryAdapter(
    IMapper<SystemPropertiesEntity, SystemPropertiesMongoEntity> domainEntityToDataEntityMapper,
    IMapper<SystemPropertiesMongoEntity, SystemPropertiesEntity> dataEntityToDomainEntityMapper,
    IMapper<SystemPropertiesId, long> domainIdentityToDataIdentityMapper,
    IMapper<long, SystemPropertiesId> dataIdentityToDomainIdentityMapper,
    ISystemPropertiesMongoRepository dataRepository) : base(
    domainEntityToDataEntityMapper,
    dataEntityToDomainEntityMapper,
    domainIdentityToDataIdentityMapper,
    dataIdentityToDomainIdentityMapper,
    dataRepository)
  {
  }
}
