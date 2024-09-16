using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class BranchCreationCommandValidator : AbstractValidator<BranchCreationCommand>
{
  public BranchCreationCommandValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}
