using CSharpFunctionalExtensions;

namespace TitanFitness.Domain.ValueObjects;

public sealed class Phone
{
    private Phone(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Phone, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success<Phone, Error>(null!);

        if (value.Length > 20)
            return Result.Failure<Phone, Error>(
                Error.Validation<Phone>("Phone cannot exceed 20 characters."));

        return Result.Success<Phone, Error>(
            new Phone(value));
    }
}
