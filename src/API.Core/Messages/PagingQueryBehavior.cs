using FluentValidation;
using FluentValidation.Results;
using Fossa.API.Core.Messages.Queries;
using Microsoft.Extensions.Options;
using TIKSN.Configuration;

namespace Fossa.API.Core.Messages;

public class PagingQueryBehavior<TRequest, TResponse>
  : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IPagingQuery
  where TResponse : notnull
{
  private readonly IOptions<PagingQueryOptions> _options;

  public PagingQueryBehavior(IOptions<PagingQueryOptions> options)
  {
    _options = options ?? throw new ArgumentNullException(nameof(options));
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    _options.Value.MaximumPageSize
      .ToOption()
      .Match(maxPageSize =>
        {
          if (maxPageSize < 1)
          {
            throw new ConfigurationValidationException(
              "Maximum Page Size for Paging Query is misconfigured, it should be greater than 1");
          }

          if (request.Page.Size > maxPageSize)
          {
            throw new ValidationException(
              Seq1(new ValidationFailure(
                "Page.Size",
                "Page Size should be less than or equal to Maximum Page Size",
                request.Page.Size)));
          }
        },
        () => throw new ConfigurationValidationException("Maximum Page Size for Paging Query is missing"));

    return await next(cancellationToken).ConfigureAwait(false);
  }
}
