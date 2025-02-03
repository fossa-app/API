using FluentValidation;

namespace Fossa.API.Core.Extensions;

public static class ParserResultExtensions
{
  public static T GetOrThrow<T>(this ParserResult<T> parserResult)
  {
    return parserResult.Match(
      e => throw CreateValidationException(e),
      c => throw CreateValidationException(c),
      o => o.Reply.Result);
  }

  private static ValidationException CreateValidationException(ParserError error)
  {
    return new ValidationException(error.Msg);
  }
}
