using Fossa.API.Core.Repositories;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public class LicenseMongoFileRepository : MongoFileRepository<Guid, object>, ILicenseFileRepository
{
  public LicenseMongoFileRepository(IMongoDatabaseProvider mongoDatabaseProvider)
    : base(mongoDatabaseProvider, "License")
  {
  }
}
