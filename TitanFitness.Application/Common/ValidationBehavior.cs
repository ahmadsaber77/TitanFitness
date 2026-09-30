using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using TitanFitness.Domain;

namespace TitanFitness.Application.Common;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(
                validator => validator.ValidateAsync(
                    context,
                    cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var message = string.Join(
            " | ",
            failures.Select(x => x.ErrorMessage));

        var error = Error.Validation<TRequest>(message);

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition()
                == typeof(Result<,>))
        {
            var resultType = typeof(TResponse);

            var valueType = resultType.GetGenericArguments()[0];

            var failureMethod = typeof(Result)
                .GetMethods()
                .First(x =>
                    x.Name == nameof(Result.Failure) &&
                    x.IsGenericMethodDefinition &&
                    x.GetGenericArguments().Length == 2 &&
                    x.GetParameters().Length == 1);

            var genericFailureMethod =
                failureMethod.MakeGenericMethod(
                    valueType,
                    typeof(Error));

            return (TResponse)genericFailureMethod.Invoke(
                null,
                new object[] { error })!;
        }

        return await next();
    }
}