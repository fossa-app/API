using Fossa.API.Core.Entities;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Messages.Commands;

public class DepartmentModificationCommandHandler : IRequestHandler<DepartmentModificationCommand, Unit>
{
  private readonly IDepartmentQueryRepository _departmentQueryRepository;
  private readonly IDepartmentRepository _departmentRepository;

  public DepartmentModificationCommandHandler(
      IDepartmentQueryRepository departmentQueryRepository,
      IDepartmentRepository departmentRepository)
  {
    _departmentQueryRepository = departmentQueryRepository ?? throw new ArgumentNullException(nameof(departmentQueryRepository));
    _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
  }

  public async Task<Unit> Handle(
      DepartmentModificationCommand request,
      CancellationToken cancellationToken)
  {
    var entity = await _departmentQueryRepository.GetAsync(request.ID, cancellationToken).ConfigureAwait(false);
    entity = entity with
    {
      Name = request.Name,
      ParentDepartmentId = request.ParentDepartmentId,
      ManagerId = request.ManagerId
    };
    await _departmentRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
    return Unit.Value;
  }
}
