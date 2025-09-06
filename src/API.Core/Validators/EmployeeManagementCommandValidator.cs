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
  private readonly IEmployeeQueryRepository _employeeQueryRepository;

  public EmployeeManagementCommandValidator(
      IBranchQueryRepository branchQueryRepository,
      IDepartmentQueryRepository departmentQueryRepository,
      IEmployeeQueryRepository employeeQueryRepository)
  {
    _branchQueryRepository = branchQueryRepository;
    _departmentQueryRepository = departmentQueryRepository;
    _employeeQueryRepository = employeeQueryRepository ?? throw new ArgumentNullException(nameof(employeeQueryRepository));

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

    RuleFor(x => x.ReportsToId)
      .MustAsync(ValidateReportsToIdBasicAsync)
      .WithMessage("ReportsToId must exist and belong to the same tenant");

    RuleFor(x => x.ReportsToId)
      .Must((command, reportsToId) =>
        reportsToId.Match(
          Some: id => id != command.ID,
          None: () => true))
      .WithMessage("An employee cannot report to themselves.");

    RuleFor(x => x.ReportsToId)
      .MustAsync(ValidateReportsToIdNoCyclesAsync)
      .WithMessage("ReportsToId must exist and belong to the same tenant");
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

  private async Task<bool> ValidateReportsToIdBasicAsync(EmployeeManagementCommand command, Option<EmployeeId> reportsToId, CancellationToken cancellationToken)
  {
    return await reportsToId.Match(
        Some: async id =>
        {
          var reportsToEmployee = await _employeeQueryRepository.GetOrNoneAsync(id, cancellationToken).ConfigureAwait(false);
          return reportsToEmployee.Match(
                  Some: x => x.TenantID == command.TenantID,
                  None: () => false);
        },
        None: () => Task.FromResult(true)).ConfigureAwait(false);
  }

  private async Task<bool> ValidateReportsToIdNoCyclesAsync(EmployeeManagementCommand command, Option<EmployeeId> reportsToId, CancellationToken cancellationToken)
  {
    return await reportsToId.Match(
        Some: async id =>
        {
          var visited = new System.Collections.Generic.HashSet<EmployeeId> { command.ID };
          return await VisitAsync(id, visited, cancellationToken).ConfigureAwait(false);
        },
        None: () => Task.FromResult(true)).ConfigureAwait(false);

    async Task<bool> VisitAsync(EmployeeId reportsToId, ISet<EmployeeId> visited, CancellationToken cancellationToken)
    {
      if (!visited.Add(reportsToId))
        return false;

      var upperManager = await _employeeQueryRepository.GetAsync(reportsToId, cancellationToken).ConfigureAwait(false);
      return await upperManager.ReportsToId.MatchAsync(
          Some: upperManagerReportsToId => VisitAsync(upperManagerReportsToId, visited, cancellationToken),
          None: () => true).ConfigureAwait(false);
    }
  }
}
