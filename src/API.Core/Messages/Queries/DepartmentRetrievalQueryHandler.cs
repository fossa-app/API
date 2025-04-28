using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class DepartmentRetrievalQueryHandler : IRequestHandler<DepartmentRetrievalQuery, DepartmentEntity>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;

  public DepartmentRetrievalQueryHandler(IDepartmentQueryRepository departmentQueryRepository)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
  }

  public Task<DepartmentEntity> Handle(
      DepartmentRetrievalQuery request,
      CancellationToken cancellationToken)
  {
    return _departmentQueryRepository.GetAsync(request.ID, cancellationToken);
  }
}
