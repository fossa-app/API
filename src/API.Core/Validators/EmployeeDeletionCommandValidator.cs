using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class EmployeeDeletionCommandValidator : AbstractValidator<EmployeeDeletionCommand>
{
  public EmployeeDeletionCommandValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
  }
}
