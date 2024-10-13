using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class BranchDeletionCommandValidator : AbstractValidator<BranchDeletionCommand>
{
  public BranchDeletionCommandValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
  }
}
