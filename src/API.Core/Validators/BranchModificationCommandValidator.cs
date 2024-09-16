using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class BranchModificationCommandValidator : AbstractValidator<BranchModificationCommand>
{
  public BranchModificationCommandValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}
