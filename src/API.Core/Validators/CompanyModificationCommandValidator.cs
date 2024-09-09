using FluentValidation;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Validators;

public class CompanyModificationCommandValidator : AbstractValidator<CompanyModificationCommand>
{
  public CompanyModificationCommandValidator(ICompanyQueryRepository companyQueryRepository)
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}
