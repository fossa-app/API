using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class CurrentEmployeeRetrievalQueryHandler : IRequestHandler<CurrentEmployeeRetrievalQuery, EmployeeEntity>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;

  public CurrentEmployeeRetrievalQueryHandler(
    IEmployeeQueryRepository employeeQueryRepository)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
  }

  public async Task<EmployeeEntity> Handle(
    CurrentEmployeeRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    return await _employeeQueryRepository.GetByUserIdAsync(
        request.UserID, cancellationToken)
      .ConfigureAwait(false);
  }
}
