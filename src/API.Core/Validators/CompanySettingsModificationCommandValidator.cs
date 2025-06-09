using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class CompanySettingsModificationCommandValidator : AbstractValidator<CompanySettingsModificationCommand>
{
  public CompanySettingsModificationCommandValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.ColorSchemeId).NotEmpty();
  }
}
