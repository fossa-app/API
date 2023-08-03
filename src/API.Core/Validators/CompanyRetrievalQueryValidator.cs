using FluentValidation;
using Fossa.API.Core.Messages.Queries;

namespace Fossa.API.Core.Validators;

public class CompanyRetrievalQueryValidator : AbstractValidator<CompanyRetrievalQuery>
{
  public CompanyRetrievalQueryValidator()
  {
    RuleFor(x => x.TenantID).NotEmpty();
  }
}
