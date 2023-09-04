using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface ISystemPropertiesMongoRepository : IMongoRepository<SystemPropertiesMongoEntity, long>
{
}
