﻿using EasyDoubles;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using TIKSN.Data;

namespace Fossa.API.FunctionalTests.Repositories;

public class EmployeeMongoEasyRepository
  : EasyRepository<EmployeeMongoEntity, long>
  , IEmployeeMongoRepository
{
  public EmployeeMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task EnsureIndexesCreatedAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task<Option<EmployeeMongoEntity>> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    return Task.FromResult(Optional(EasyStore.Entities.Values.FirstOrDefault(x => x.UserID == userId)));
  }

  public Task<EmployeeMongoEntity> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var entity = EasyStore.Entities.Values.FirstOrDefault(x => x.UserID == userId);
    if (entity is null)
    {
      return Task.FromException<EmployeeMongoEntity>(new EntityNotFoundException());
    }

    return Task.FromResult(entity);
  }

  public Task<bool> HasDependencyOnBranchAsync(long branchId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.AssignedBranchId == branchId));
  }

  public Task<bool> HasDependencyOnCompanyAsync(long companyId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.CompanyId == companyId));
  }

  public Task<bool> HasDependencyOnDepartmentAsync(long departmentId, CancellationToken cancellationToken)
  {
    return Task.FromResult(EasyStore.Entities.Values.Any(x => x.AssignedDepartmentId == departmentId));
  }

  public Task<PageResult<EmployeeMongoEntity>> PageAsync(TenantEmployeePageQuery pageQuery, CancellationToken cancellationToken)
  {
    var allItems = EasyStore.Entities.Values
      .Where(x => x.TenantID == pageQuery.TenantId && (string.IsNullOrEmpty(pageQuery.Search)
      || x.FirstName?.Contains(pageQuery.Search, StringComparison.OrdinalIgnoreCase) == true
      || x.LastName?.Contains(pageQuery.Search, StringComparison.OrdinalIgnoreCase) == true
      || x.FullName?.Contains(pageQuery.Search, StringComparison.OrdinalIgnoreCase) == true))
      .ToList();

    var pageItems = allItems.Skip((pageQuery.Page.Number - 1) * pageQuery.Page.Size).Take(pageQuery.Page.Size).ToList();

    return Task.FromResult(new PageResult<EmployeeMongoEntity>(pageQuery.Page, pageItems, allItems.Length()));
  }

  public Task<int> CountAllAsync(long companyId, CancellationToken cancellationToken)
  {
    return Task.FromResult(
      EasyStore.Entities
        .Count(x => x.Value.CompanyId == companyId));
  }
}
