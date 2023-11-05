﻿using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record EmployeeEntity(
    EmployeeId ID,
    Guid TenantID,
    Guid UserID,
    CompanyId CompanyId,
    string FirstName,
    string LastName,
    string FullName)
  : ITenantEntity<EmployeeId, Guid>, IUserEntity<EmployeeId, Guid>;
