using Ardalis.Specification.EntityFrameworkCore;
using Fossa.API.SharedKernel.Interfaces;

namespace Fossa.API.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  public EfRepository(AppDbContext dbContext) : base(dbContext)
  {
  }
}
