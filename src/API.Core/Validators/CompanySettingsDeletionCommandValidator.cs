using FluentValidation;
using Fossa.API.Core.Messages.Commands;

namespace Fossa.API.Core.Validators;

public class CompanySettingsDeletionCommandValidator : AbstractValidator<CompanySettingsDeletionCommand>
{
  public CompanySettingsDeletionCommandValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
    RuleFor(x => x.ID).NotEmpty();
  }
}
