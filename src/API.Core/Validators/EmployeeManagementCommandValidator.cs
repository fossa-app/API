using FluentValidation;
using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;
using TIKSN.Data;

namespace Fossa.API.Core.Validators;

public class EmployeeManagementCommandValidator : AbstractValidator<EmployeeManagementCommand>
{
  private readonly IBranchQueryRepository _branchQueryRepository;
  private readonly IDepartmentQueryRepository _departmentQueryRepository;

  public EmployeeManagementCommandValidator(
      IBranchQueryRepository branchQueryRepository,
      IDepartmentQueryRepository departmentQueryRepository)
  {
    _branchQueryRepository = branchQueryRepository;
    _departmentQueryRepository = departmentQueryRepository;

    RuleFor(x => x.TenantID)
        .NotEmpty();

    RuleFor(x => x.UserID)
        .NotEmpty();

    RuleFor(x => x.AssignedBranchId)
        .MustAsync(ValidateBranchAsync)
        .WithMessage("Branch must exist and belong to the same tenant");

    RuleFor(x => x.AssignedDepartmentId)
        .MustAsync(ValidateDepartmentAsync)
        .WithMessage("Department must exist and belong to the same tenant");
  }

  private async Task<bool> ValidateBranchAsync(EmployeeManagementCommand command, Option<BranchId> assignedBranchId, CancellationToken cancellationToken)
  {
    return await assignedBranchId.Match(
        Some: async id =>
        {
          var branch = await _branchQueryRepository.GetOrNoneAsync(id, cancellationToken).ConfigureAwait(false);
          return branch.Match(
                  Some: x => x.TenantID == command.TenantID,
                  None: () => false);
        },
        None: () => Task.FromResult(true)).ConfigureAwait(false);
  }

  private async Task<bool> ValidateDepartmentAsync(EmployeeManagementCommand command, Option<DepartmentId> assignedDepartmentId, CancellationToken cancellationToken)
  {
    return await assignedDepartmentId.Match(
        Some: async id =>
        {
          var department = await _departmentQueryRepository.GetOrNoneAsync(id, cancellationToken).ConfigureAwait(false);
          return department.Match(
                  Some: x => x.TenantID == command.TenantID,
                  None: () => false);
        },
        None: () => Task.FromResult(true)).ConfigureAwait(false);
  }
}
