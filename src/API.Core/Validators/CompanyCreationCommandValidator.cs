using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class CompanyCreationCommandValidator : AbstractValidator<CompanyCreationCommand>
{
  public CompanyCreationCommandValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}
