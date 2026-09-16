using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Enums;

namespace TitanFitness.Domain.Entities;
public class Plan : IAggregateRoot
{


    private Plan()
    {
    }

    private Plan(
        Guid id,
        string name,
        decimal price,
        int durationInMonths,
        int maxFreezeDays,
        int maxNumberOfFreezes,
        int guestPassQuota,
        AccessScope accessScope,
        bool isPublished)
    {
        Id = id;
        Name = name;
        Price = price;
        DurationInMonths = durationInMonths;
        MaxFreezeDays = maxFreezeDays;
        MaxNumberOfFreezes = maxNumberOfFreezes;
        GuestPassQuota = guestPassQuota;
        AccessScope = accessScope;
        IsPublished = isPublished;
    }

    public static Result<Plan, Error> Create(
        string name,
        decimal price,
        int durationInMonths,
        int maxFreezeDays,
        int maxNumberOfFreezes,
        int guestPassQuota,
        AccessScope accessScope,
        bool isPublished)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan name is required."));

        if (name.Length > 50)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan name cannot exceed 50 characters."));

        if (price < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan price cannot be negative."));

        if (decimal.Round(price, 2) != price)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan price cannot have more than 2 decimal places."));

        if (durationInMonths < 1)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan duration must be at least 1 month."));

        if (maxFreezeDays < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Maximum freeze days cannot be negative."));

        if (maxNumberOfFreezes < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Maximum number of freezes cannot be negative."));

        if (guestPassQuota < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Guest pass quota cannot be negative."));

        if (!Enum.IsDefined(accessScope))
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Invalid access scope."));

        var plan = new Plan(
            Guid.NewGuid(),
            name,
            price,
            durationInMonths,
            maxFreezeDays,
            maxNumberOfFreezes,
            guestPassQuota,
            accessScope,
            isPublished);

        return Result.Success<Plan, Error>(plan);
    }


    public Result<bool, Error> Publish()
    {
        if (IsPublished)
            return Result.Failure<bool, Error>(
                Error.Conflict<Plan>(
                    "Plan is already published."));

        IsPublished = true;

        return Result.Success<bool, Error>(true);
    }



    public Result<bool, Error> Unpublish()
    {
        if (!IsPublished)
            return Result.Failure<bool, Error>(
                Error.Conflict<Plan>(
                    "Plan is already unpublished."));

        IsPublished = false;

        return Result.Success<bool, Error>(true);
    }


    public Result<Plan, Error> Update(
    string name,
    decimal price,
    int durationInMonths,
    int maxFreezeDays,
    int maxNumberOfFreezes,
    int guestPassQuota,
    AccessScope accessScope)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan name is required."));

        if (name.Length > 50)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan name cannot exceed 50 characters."));

        if (price < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Plan price cannot be negative."));


        if (decimal.Round(price, 2) != price)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Price cannot have more than 2 decimal places."));

        if (durationInMonths < 1)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Duration must be at least 1 month."));

        if (maxFreezeDays < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Maximum freeze days cannot be negative."));

        if (maxNumberOfFreezes < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Maximum number of freezes cannot be negative."));

        if (guestPassQuota < 0)
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Guest pass quota cannot be negative."));

        if (!Enum.IsDefined(accessScope))
            return Result.Failure<Plan, Error>(
                Error.Validation<Plan>(
                    "Invalid access scope."));

        Name = name;
        Price = price;
        DurationInMonths = durationInMonths;
        MaxFreezeDays = maxFreezeDays;
        MaxNumberOfFreezes = maxNumberOfFreezes;
        GuestPassQuota = guestPassQuota;
        AccessScope = accessScope;

        return Result.Success<Plan, Error>(this);

    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int DurationInMonths { get; private set; }

    public int MaxFreezeDays { get; private set; }

    public int MaxNumberOfFreezes { get; private set; }

    public int GuestPassQuota { get; private set; }

    public AccessScope AccessScope { get; private set; }

    public bool IsPublished { get; private set; }
}
