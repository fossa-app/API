using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class EmployeeModificationCommandValidator : AbstractValidator<EmployeeModificationCommand>
{
  public EmployeeModificationCommandValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.FirstName).NotEmpty();
    RuleFor(x => x.LastName).NotEmpty();
    RuleFor(x => x.FullName).NotEmpty();
  }
}
