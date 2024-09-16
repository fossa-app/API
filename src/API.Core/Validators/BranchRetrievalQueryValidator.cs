using FluentValidation;
using Fossa.API.Core.Messages.Queries;

namespace Fossa.API.Core.Validators;

public class BranchRetrievalQueryValidator : AbstractValidator<BranchRetrievalQuery>
{
  public BranchRetrievalQueryValidator()
  {
    RuleFor(x => x.ID).NotEmpty();
    RuleFor(x => x.TenantID).NotEmpty();
    RuleFor(x => x.UserID).NotEmpty();
  }
}
