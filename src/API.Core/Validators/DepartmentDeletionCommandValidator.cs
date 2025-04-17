using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class DepartmentDeletionCommandValidator : AbstractValidator<DepartmentDeletionCommand>
{
  public DepartmentDeletionCommandValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
  }
}
