using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Fossa.API.Core.Messages;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  private static readonly Type _validationBehaviorType = typeof(ValidationBehavior<,>);
  private static readonly Type _validatorType = typeof(IValidator<>);

  private readonly IServiceProvider _serviceProvider;
  private readonly Seq<IValidator<TRequest>> _requestValidators;
  private readonly Seq<IValidator<TResponse>> _responseValidators;

  public ValidationBehavior(
    IEnumerable<IValidator<TRequest>> requestValidators,
    IEnumerable<IValidator<TResponse>> responseValidators,
    IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    _requestValidators = requestValidators?.ToSeq() ?? throw new ArgumentNullException(nameof(requestValidators));
    _responseValidators = responseValidators?.ToSeq() ?? throw new ArgumentNullException(nameof(responseValidators));
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    foreach (var requestInterfaceType in request.GetType().GetInterfaces())
    {
      await EnsureValidityAsync(request, requestInterfaceType, _serviceProvider, cancellationToken).ConfigureAwait(false);
    }

    await EnsureValidityAsync(_requestValidators, request, cancellationToken).ConfigureAwait(false);

    var response = await next(cancellationToken).ConfigureAwait(false);

    await EnsureValidityAsync(_responseValidators, response, cancellationToken).ConfigureAwait(false);

    foreach (var responseInterfaceType in request.GetType().GetInterfaces())
    {
      await EnsureValidityAsync(response, responseInterfaceType, _serviceProvider, cancellationToken).ConfigureAwait(false);
    }

    return response;
  }

  private static Task EnsureValidityAsync<TR>(
    TR instanceToValidate,
    Type interfaceType,
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken)
  {
#pragma warning disable S3011
    var ensureValidityMethod = _validationBehaviorType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
#pragma warning restore S3011
      .Single(x => x.Name == nameof(EnsureValidityAsync) && x.GetGenericArguments().Length == 2);

    var genericEnsureValidityMethod = ensureValidityMethod.MakeGenericMethod(typeof(TR), interfaceType);

    if (genericEnsureValidityMethod.ContainsGenericParameters)
    {
      return Task.CompletedTask;
    }

    var validatorType = _validatorType.MakeGenericType(interfaceType);
    var validators = serviceProvider.GetServices(validatorType).ToSeq();

    var validationResult = genericEnsureValidityMethod.Invoke(null, new object?[]
    {
      validators,
      instanceToValidate,
      cancellationToken
    });

    if (validationResult is Task task)
    {
      return task;
    }

    throw new InvalidOperationException("Validation result is not Task");

  }
  private static async Task EnsureValidityAsync<TR, TV>(
    Seq<IValidator<TV>> validators,
    TR instanceToValidate,
    CancellationToken cancellationToken)
  {
    if (validators.Any())
    {
      var context = new ValidationContext<TR>(instanceToValidate);
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
