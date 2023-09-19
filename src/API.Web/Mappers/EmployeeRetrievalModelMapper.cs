using Fossa.API.Core.Entities;
using Fossa.API.Web.ApiModels;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class EmployeeRetrievalModelMapper : IMapper<EmployeeEntity, EmployeeRetrievalModel>
{
  public EmployeeRetrievalModel Map(EmployeeEntity source)
  {
    return new EmployeeRetrievalModel(
      source.ID, source.CompanyId,
      source.FirstName, source.LastName, source.FullName);
  }
}
