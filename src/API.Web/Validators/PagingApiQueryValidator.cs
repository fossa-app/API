using FluentValidation;
using Fossa.API.Web.Messages.Queries;

namespace Fossa.API.Web.Validators;

public class PagingApiQueryValidator : AbstractValidator<IPagingApiQuery>
{
  public PagingApiQueryValidator()
  {
    RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
    RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
  }
}
