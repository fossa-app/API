﻿using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record BranchEntity(
    BranchId ID,
    Guid TenantID,
    CompanyId CompanyId,
    string Name)
  : ITenantEntity<BranchId, Guid>;
