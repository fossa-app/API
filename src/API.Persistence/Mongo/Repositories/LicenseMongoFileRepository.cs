using Fossa.API.Core.Repositories;
using MongoDB.Bson;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class LicenseMongoFileRepository : MongoFileRepository<ObjectId, object>, ILicenseFileRepository
{
  public LicenseMongoFileRepository(IMongoDatabaseProvider mongoDatabaseProvider)
    : base(mongoDatabaseProvider, "License")
  {
  }
}
