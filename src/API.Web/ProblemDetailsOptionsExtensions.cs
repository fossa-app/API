using FluentValidation;
using Fossa.API.Core;
using Fossa.API.Core.Messages;
using Hellang.Middleware.ProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Fossa.API.Web;

public static class ProblemDetailsOptionsExtensions
{
  public static void MapFluentValidationException(
    this ProblemDetailsOptions options) =>
      options.Map<ValidationException>((ctx, ex) =>
      {
        var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var errors = ex.Errors
                  .GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
                  .ToDictionary(
                      x => x.Key,
                      x => x.Select(x => x.ErrorMessage).ToArray(), StringComparer.OrdinalIgnoreCase);

        return factory.CreateValidationProblemDetails(ctx, errors);
      });

  public static void MapKnownExceptions(
      this ProblemDetailsOptions options)
  {
    options.MapToStatusCode<EntityNotFoundException>(StatusCodes.Status404NotFound);
    options.MapToStatusCode<CrossTenantInboundUnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
    options.MapToStatusCode<CrossTenantOutboundUnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
    options.MapToStatusCode<CrossTenantUnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
  }
}
