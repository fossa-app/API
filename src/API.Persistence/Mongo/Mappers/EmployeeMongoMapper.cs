using Fossa.API.Core.Entities;
using Fossa.API.Persistence.Mongo.Entities;
using TIKSN.Mapping;

namespace Fossa.API.Persistence.Mongo.Mappers;

public class EmployeeMongoMapper
  : IMapper<EmployeeMongoEntity, EmployeeEntity>
  , IMapper<EmployeeEntity, EmployeeMongoEntity>
{
  public EmployeeEntity Map(EmployeeMongoEntity source)
  {
    return new(
      source.ID,
      source.TenantID,
      source.UserID,
      source.CompanyId,
      source?.FirstName ?? string.Empty,
      source?.LastName ?? string.Empty,
      source?.FullName ?? string.Empty);
  }

  public EmployeeMongoEntity Map(EmployeeEntity source)
  {
    return new EmployeeMongoEntity
    {
      ID = source.ID,
      TenantID = source.TenantID,
      UserID = source.UserID,
      CompanyId = source.CompanyId,
      FirstName = source.FirstName,
      LastName = source.LastName,
      FullName = source.FullName,
    };
  }
}
