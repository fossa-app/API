using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class CompanySettingsCreationCommandValidator : AbstractValidator<CompanySettingsCreationCommand>
{
  public CompanySettingsCreationCommandValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.ColorSchemeId).NotEmpty();
  }
}
