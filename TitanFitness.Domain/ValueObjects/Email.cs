using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace TitanFitness.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<Email?, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success<Email?, Error>(null);

        if (value.Length > 100)
            return Result.Failure<Email?, Error>(
                Error.Validation<Email>(
                    "Email cannot exceed 100 characters."));

        if (!Regex.IsMatch(
                value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return Result.Failure<Email?, Error>(
                Error.Validation<Email>(
                    "Invalid email format."));
        }

        return Result.Success<Email?, Error>(
            new Email(value));
    }
}
