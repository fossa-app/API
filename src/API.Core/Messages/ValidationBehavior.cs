using FluentValidation;

namespace Fossa.API.Core.Messages;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  private readonly Seq<IValidator<TRequest>> _requestValidators;
  private readonly Seq<IValidator<TResponse>> _responseValidators;

  public ValidationBehavior(
    IEnumerable<IValidator<TRequest>> requestValidators,
    IEnumerable<IValidator<TResponse>> responseValidators)
  {
    _requestValidators = requestValidators?.ToSeq() ?? throw new ArgumentNullException(nameof(requestValidators));
    _responseValidators = responseValidators?.ToSeq() ?? throw new ArgumentNullException(nameof(responseValidators));
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    await EnsureValidityAsync(_requestValidators, request, cancellationToken).ConfigureAwait(false);

    var response = await next(cancellationToken).ConfigureAwait(false);

    await EnsureValidityAsync(_responseValidators, response, cancellationToken).ConfigureAwait(false);

    return response;
  }

  private static async Task EnsureValidityAsync<T>(
    Seq<IValidator<T>> validators,
    T instanceToValidate,
    CancellationToken cancellationToken)
  {
    if (validators.Any())
    {
      var context = new ValidationContext<T>(instanceToValidate);
      var validationResults = await Task
        .WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
      var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
      if (failures.Count != 0)
      {
        throw new ValidationException(failures);
      }
    }
  }
}
