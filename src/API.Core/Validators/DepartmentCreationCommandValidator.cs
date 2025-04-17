using FluentValidation;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Validators;

public class DepartmentCreationCommandValidator : AbstractValidator<DepartmentCreationCommand>
{
  public DepartmentCreationCommandValidator(
      IDepartmentQueryRepository departmentQueryRepository,
      IEmployeeQueryRepository employeeQueryRepository)
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.ManagerId)
        .MustAsync(async (command, managerId, cancellationToken) =>
        {
          var employee = await employeeQueryRepository.GetOrNoneAsync(managerId, cancellationToken);
          return employee.Match(e => e.TenantID == command.TenantID, () => false);
        })
        .WithMessage("Manager must be an employee in the same tenant");

    _ = RuleFor(x => x.ParentDepartmentId)
        .MustAsync(async (command, parentId, cancellationToken) =>
        {
          var foundParentDepartment = parentId.MapAsync(id => departmentQueryRepository.GetOrNoneAsync(id, cancellationToken));
          return await foundParentDepartment.Match(
              d => d.Match(
                  department => department.TenantID == command.TenantID,
                  () => false),
              () => true);
        })
        .WithMessage("Parent department must be in the same tenant");
  }
}
