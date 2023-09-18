using Fossa.API.Persistence.Mongo.Entities;
using LanguageExt;
using TIKSN.Data.Mongo;

namespace Fossa.API.Persistence.Mongo.Repositories;

public interface IEmployeeMongoRepository : IMongoRepository<EmployeeMongoEntity, long>
{
  Task<Option<EmployeeMongoEntity>> FindByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken);

  Task<EmployeeMongoEntity> GetByUserIdAsync(
    Guid userId,
    CancellationToken cancellationToken);
}
