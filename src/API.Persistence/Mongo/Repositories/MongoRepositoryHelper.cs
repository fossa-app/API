using MongoDB.Driver;

namespace Fossa.API.Persistence.Mongo.Repositories;

public static class MongoRepositoryHelper
{
  public static Collation CreateDefaultCollation()
  {
    return new Collation(
      "simple",
      strength: CollationStrength.Primary,
      alternate: CollationAlternate.Shifted);
  }
}
