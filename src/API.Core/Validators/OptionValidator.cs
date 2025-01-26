using FluentValidation;
using FluentValidation.Validators;

namespace Fossa.API.Core.Validators;

public class OptionValidator<T, TProperty> : PropertyValidator<T, Option<TProperty>>
{
  private readonly IValidator<TProperty> _validator;

  public OptionValidator(IValidator<TProperty> validator)
  {
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
  }

  public override string Name => "OptionValidator";

  public override bool IsValid(ValidationContext<T> context, Option<TProperty> value)
  {
    return value.Match(
      v => _validator.Validate(v).IsValid,
      true);
  }
}
