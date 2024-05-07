using FluentValidation;

namespace Fossa.API.Core.Extensions;

public static class MonikerValidationExtensions
{
  public static IRuleBuilderOptions<T, string> SubDomainLength<T>(this IRuleBuilder<T, string> ruleBuilder) =>
    ruleBuilder
      .Length(3, 63);

  public static IRuleBuilderOptions<T, string> OnlyAsciiLetterLowersAndDigits<T>(this IRuleBuilder<T, string> ruleBuilder) =>
    ruleBuilder
      .Must((_, moniker, _) => moniker.ForAll(c => char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c)))
      .WithMessage("{PropertyName} must contain only ASCII lower letters or ASCII digits.");
}
