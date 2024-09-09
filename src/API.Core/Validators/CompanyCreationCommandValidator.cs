using FluentValidation;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Repositories;

namespace Fossa.API.Core.Validators;

public class CompanyCreationCommandValidator : AbstractValidator<CompanyCreationCommand>
{
  public CompanyCreationCommandValidator(ICompanyQueryRepository companyQueryRepository)
  {
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}
