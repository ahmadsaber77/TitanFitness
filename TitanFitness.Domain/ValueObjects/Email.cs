using CSharpFunctionalExtensions;

namespace TitanFitness.Domain.ValueObjects;

public sealed class Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success<Email, Error>(null!);

        if (value.Length > 100)
            return Result.Failure<Email, Error>(
                Error.Validation<Email>("Email cannot exceed 100 characters."));

        return Result.Success<Email, Error>(
            new Email(value));
    }
}
