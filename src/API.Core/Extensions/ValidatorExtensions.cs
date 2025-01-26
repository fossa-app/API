using FluentValidation;
using Fossa.API.Core.Validators;

namespace Fossa.API.Core.Extensions;

public static partial class ValidatorExtensions
{
  public static IRuleBuilderOptions<T, Option<TProperty>> IfSome<T, TProperty>(
   this IRuleBuilder<T, Option<TProperty>> ruleBuilder,
   IValidator<TProperty> validator)
  {
    return ruleBuilder.SetValidator(new OptionValidator<T, TProperty>(validator));
  }
}
