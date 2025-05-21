using System.Globalization;
using FluentValidation;
using FluentValidation.Results;

namespace Fossa.API.Core.Extensions;

public static class ValidationExtensions
{
  public static T GetOrThrow<T>(this Validation<Error, T> validation)
  {
    return validation.Match(s => s, f => throw CreateValidationException(f));
  }

  private static ValidationException CreateValidationException(Seq<Error> errors)
  {
    return new ValidationException(CreateValidationFailures(errors));
  }

  private static IEnumerable<ValidationFailure> CreateValidationFailures(Seq<Error> errors)
  {
    return errors
      .SelectMany(error => CreateValidationFailures(error, None));
  }

  private static IEnumerable<ValidationFailure> CreateValidationFailures(Error error, Option<string> parentPropertyName)
  {
    ArgumentNullException.ThrowIfNull(error);

    return CreateValidationFailures(error, parentPropertyName);
    static IEnumerable<ValidationFailure> CreateValidationFailures(Error error, Option<string> parentErrorCodePath)
    {
      var errorCode = error.Code.ToString(CultureInfo.InvariantCulture);
      var errorCodePath = parentErrorCodePath.Match(
        p => $"{p}.E{errorCode}", $"E{errorCode}");
      var errorMessage = $"{errorCodePath}: {error.Message}";
      yield return new ValidationFailure(string.Empty, errorMessage) { ErrorCode = errorCode };

      if (error is ManyErrors manyErrors)
      {
        foreach (var oneValidationFailure in manyErrors.Errors
          .Map(e => CreateValidationFailures(e, Some(errorCodePath)))
          .SelectMany(x => x))
        {
          yield return oneValidationFailure;
        }
      }

      foreach (var innerValidationFailure in error.Inner
        .Map(e => CreateValidationFailures(e, Some(errorCodePath)))
        .SelectMany(x => x))
      {
        yield return innerValidationFailure;
      }
    }
  }
}
