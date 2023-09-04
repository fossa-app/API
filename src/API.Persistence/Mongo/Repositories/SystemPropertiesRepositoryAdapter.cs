using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data.Mongo;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class SystemPropertiesRepositoryAdapter
  : MongoRepositoryAdapter<SystemPropertiesEntity, long, SystemPropertiesMongoEntity, long>
    , ISystemPropertiesRepository, ISystemPropertiesQueryRepository
{
  private readonly ISystemPropertiesMongoRepository _dataRepository;

  public SystemPropertiesRepositoryAdapter(
    IMapper<SystemPropertiesEntity, SystemPropertiesMongoEntity> domainEntityToDataEntityMapper,
    IMapper<SystemPropertiesMongoEntity, SystemPropertiesEntity> dataEntityToDomainEntityMapper,
    ISystemPropertiesMongoRepository dataRepository) : base(
    domainEntityToDataEntityMapper,
    dataEntityToDomainEntityMapper,
    IdentityMapper<long>.Instance,
    IdentityMapper<long>.Instance,
    dataRepository)
  {
    _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
  }
}
