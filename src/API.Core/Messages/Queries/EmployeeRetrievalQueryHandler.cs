﻿using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Queries;

public class EmployeeRetrievalQueryHandler : IRequestHandler<EmployeeRetrievalQuery, EmployeeEntity>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;

  public EmployeeRetrievalQueryHandler(
    IEmployeeQueryRepository employeeQueryRepository)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
  }

  public async Task<EmployeeEntity> Handle(
    EmployeeRetrievalQuery request,
    CancellationToken cancellationToken)
  {
    return await _employeeQueryRepository.GetAsync(
        request.ID, cancellationToken)
      .ConfigureAwait(false);
  }
}
