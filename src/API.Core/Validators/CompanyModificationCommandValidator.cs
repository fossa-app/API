using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class CompanyModificationCommandValidator : AbstractValidator<CompanyModificationCommand>
{
  public CompanyModificationCommandValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}
