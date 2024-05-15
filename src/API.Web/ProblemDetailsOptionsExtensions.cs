using FluentValidation;
using Fossa.API.Core.Messages;
using Hellang.Middleware.ProblemDetails;
using TIKSN.Data;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Fossa.API.Web;

internal static class ProblemDetailsOptionsExtensions
{
  internal static void MapFluentValidationException(
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

  internal static void MapKnownExceptions(
      this ProblemDetailsOptions options)
  {
    options.MapToStatusCode<EntityNotFoundException>(StatusCodes.Status404NotFound);
    options.MapToStatusCode<CrossTenantInboundUnauthorizedAccessException>(StatusCodes.Status403Forbidden);
    options.MapToStatusCode<CrossTenantOutboundUnauthorizedAccessException>(StatusCodes.Status403Forbidden);
    options.MapToStatusCode<CrossTenantUnauthorizedAccessException>(StatusCodes.Status403Forbidden);
  }
}
