﻿using EasyDoubles;
using Fossa.API.Core.Repositories;
using Fossa.API.Persistence.Mongo.Entities;
using Fossa.API.Persistence.Mongo.Repositories;
using LanguageExt;
using TIKSN.Data;
using static LanguageExt.Prelude;

namespace Fossa.API.FunctionalTests.Repositories;

public class BranchMongoEasyRepository :
  EasyRepository<BranchMongoEntity, long>,
  IBranchMongoRepository
{
  public BranchMongoEasyRepository(IEasyStores easyStores) : base(easyStores)
  {
  }

  public Task<PageResult<BranchMongoEntity>> PageAsync(TenantBranchPageQuery pageQuery, CancellationToken cancellationToken)
  {
    var allItems = EasyStore.Entities.Values.Where(x => x.TenantID == pageQuery.TenantId).ToList();

    var pageItems = allItems.Skip((pageQuery.Page.Number - 1) * pageQuery.Page.Size).Take(pageQuery.Page.Size).ToList();

    return Task.FromResult(new PageResult<BranchMongoEntity>(pageQuery.Page, pageItems, allItems.Length()));
  }
}
