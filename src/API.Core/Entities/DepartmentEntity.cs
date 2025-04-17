using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record DepartmentEntity(
    DepartmentId ID,
    Guid TenantID,
    CompanyId CompanyId,
    string Name,
    EmployeeId ManagerId,
    Option<DepartmentId> ParentDepartmentId)
  : ITenantEntity<DepartmentId, Guid>;
