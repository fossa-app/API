using Fossa.Bridge.Models.ApiModels;
using Fossa.Licensing;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class CompanyEntitlementsModelMapper : IMapper<CompanyEntitlements, CompanyEntitlementsModel>
{
  public CompanyEntitlementsModel Map(CompanyEntitlements source)
  {
    return new CompanyEntitlementsModel(
      source.CompanyId,
      source.MaximumBranchCount,
      source.MaximumEmployeeCount,
      source.MaximumDepartmentCount);
  }
}
