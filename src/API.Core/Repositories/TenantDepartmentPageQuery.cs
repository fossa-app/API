using TIKSN.Data;

namespace Fossa.API.Core.Repositories;

public record TenantDepartmentPageQuery(
    Guid TenantId,
    string Search,
    Page Page);
