using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeModificationCommandHandler : IRequestHandler<EmployeeModificationCommand, Unit>
{
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public EmployeeModificationCommandHandler(
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository)
  {
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
  }

  public async Task<Unit> Handle(
    EmployeeModificationCommand request,
    CancellationToken cancellationToken)
  {
    var entity = await _employeeQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);
    entity = entity with
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      FullName = request.FullName,
    };
    await _employeeRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
