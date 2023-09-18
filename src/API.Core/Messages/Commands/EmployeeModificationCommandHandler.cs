using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;
using MediatR;
using TIKSN.Identity;

namespace Fossa.API.Core.Messages.Commands;

public class EmployeeModificationCommandHandler : IRequestHandler<EmployeeModificationCommand>
{
  private readonly ICompanyQueryRepository _companyQueryRepository;
  private readonly IEmployeeQueryRepository _employeeQueryRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IIdentityGenerator<long> _identityGenerator;

  public EmployeeModificationCommandHandler(
    IIdentityGenerator<long> identityGenerator,
    ICompanyQueryRepository companyQueryRepository,
    IEmployeeQueryRepository employeeQueryRepository,
    IEmployeeRepository employeeRepository)
  {
    _identityGenerator = identityGenerator ?? throw new ArgumentNullException(nameof(identityGenerator));
    _companyQueryRepository = companyQueryRepository ?? throw new ArgumentNullException(nameof(companyQueryRepository));
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));
    _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
  }

  public async Task Handle(
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
  }
}
