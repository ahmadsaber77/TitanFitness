using CSharpFunctionalExtensions;
using TitanFitness.Domain;

namespace TitanFitness.Domain.ValueObjects;

public sealed record MembershipNumber
{
    public string Value { get; }


    private MembershipNumber(string value)
    {
        Value = value;
    }

    public static Result<MembershipNumber, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<MembershipNumber, Error>(
                Error.Validation<MembershipNumber>(
                    "Membership number cannot be empty."
                ));

        if (value.Length > 10)
            return Result.Failure<MembershipNumber, Error>(
                Error.Validation<MembershipNumber>(
                    "Membership number cannot exceed 10 characters."
                ));

        return Result.Success<MembershipNumber, Error>(
            new MembershipNumber(value));
    }
}
