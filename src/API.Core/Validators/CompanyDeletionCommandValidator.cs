using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class CompanyDeletionCommandValidator : AbstractValidator<CompanyDeletionCommand>
{
  public CompanyDeletionCommandValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
  }
}
