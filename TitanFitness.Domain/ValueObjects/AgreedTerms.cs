using CSharpFunctionalExtensions;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Domain.ValueObjects;

public sealed record AgreedTerms 
{
    private AgreedTerms() 
    {
    }


    private AgreedTerms(
       decimal price,
       int durationInMonths,
       int maxFreezeDays,
       int maxNumberOfFreezes,
       int guestPassQuota,
       AccessScope accessScope)
    {
        Price = price;
        DurationInMonths = durationInMonths;
        MaxFreezeDays = maxFreezeDays;
        MaxNumberOfFreezes = maxNumberOfFreezes;
        GuestPassQuota = guestPassQuota;
        AccessScope = accessScope;
    }

    public static Result<AgreedTerms, Error> Create(
        decimal price,
        int durationInMonths,
        int maxFreezeDays,
        int maxNumberOfFreezes,
        int guestPassQuota,
        AccessScope accessScope)
    {
        if (price < 0)
            return Result.Failure<AgreedTerms, Error>(
                Error.Validation<AgreedTerms>(
                    "Price cannot be negative."));

        if (decimal.Round(price, 2) != price)
            return Result.Failure<AgreedTerms, Error>(
                Error.Validation<AgreedTerms>(
                    "Price cannot have more than 2 decimal places."));

        if (durationInMonths < 1)
            return Result.Failure<AgreedTerms, Error>(
                Error.Validation<AgreedTerms>(
                    "Duration must be at least 1 month."));

        if (maxFreezeDays < 0)
            return Result.Failure<AgreedTerms, Error>(
                Error.Validation<AgreedTerms>(
                    "Maximum freeze days cannot be negative."));

        if (maxNumberOfFreezes < 0)
            return Result.Failure<AgreedTerms, Error>(
                Error.Validation<AgreedTerms>(
                    "Maximum number of freezes cannot be negative."));

        if (guestPassQuota < 0)
            return Result.Failure<AgreedTerms, Error>(
                Error.Validation<AgreedTerms>(
                    "Guest pass quota cannot be negative."));

        return Result.Success<AgreedTerms, Error>(
            new AgreedTerms(
                price,
                durationInMonths,
                maxFreezeDays,
                maxNumberOfFreezes,
                guestPassQuota,
                accessScope));
    }

   

    public decimal Price { get; init; }

    public int DurationInMonths { get; init; }

    public int MaxFreezeDays { get; init; }

    public int MaxNumberOfFreezes { get; init; }

    public int GuestPassQuota { get; init; }

    public AccessScope AccessScope { get; init; }
}
