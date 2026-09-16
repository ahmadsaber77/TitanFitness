using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Domain.Entities;
public class Trainer : IAggregateRoot
{
    private Trainer()
    {
    }

    private Trainer(
        Guid id,
        string trainerNumber,
        string trainerName,
        Email? email,
        Phone? phone,
        bool isActive,
        Guid branchId)
    {
        Id = id;
        TrainerNumber = trainerNumber;
        TrainerName = trainerName;
        Email = email;
        Phone = phone;
        IsActive = isActive;
        BranchId = branchId;
    }

    public static Result<Trainer, Error> Create(
        string trainerNumber,
        string trainerName,
        Email? email,
        string? phone,
        bool isActive,
        Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(trainerNumber))
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer number is required."));

        if (trainerNumber.Length > 20)
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer number cannot exceed 20 characters."));

        if (string.IsNullOrWhiteSpace(trainerName))
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer name is required."));

        if (trainerName.Length > 100)
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer name cannot exceed 100 characters."));

        if (branchId == Guid.Empty)
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Branch is required."));

        var phoneResult = Phone.Create(phone);

        if (phoneResult.IsFailure)
            return Result.Failure<Trainer, Error>(
                phoneResult.Error);

        var trainer = new Trainer(
            Guid.NewGuid(),
            trainerNumber,
            trainerName,
            email,
            phoneResult.Value,
            isActive,
            branchId);

        return Result.Success<Trainer, Error>(
            trainer);
    }

    public Result<bool, Error> Deactivate()
    {
        if (!IsActive)
            return Result.Failure<bool, Error>(
                Error.Conflict<Trainer>(
                    "Trainer is already inactive."));

        IsActive = false;

        return Result.Success<bool, Error>(true);
    }

    public Result<bool, Error> Activate()
    {
        if (IsActive)
            return Result.Failure<bool, Error>(
                Error.Conflict<Trainer>(
                    "Trainer is already active."));

        IsActive = true;

        return Result.Success<bool, Error>(true);
    }

    public Result<Trainer, Error> Update(
        string trainerNumber,
        string trainerName,
        string? email,
        string? phone,
        bool isActive,
        Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(trainerNumber))
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer number is required."));

        if (trainerNumber.Length > 20)
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer number cannot exceed 20 characters."));

        if (string.IsNullOrWhiteSpace(trainerName))
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer name is required."));

        if (trainerName.Length > 100)
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Trainer name cannot exceed 100 characters."));

        if (branchId == Guid.Empty)
            return Result.Failure<Trainer, Error>(
                Error.Validation<Trainer>(
                    "Branch is required."));

        var emailResult = Email.Create(email);

        if (emailResult.IsFailure)
            return Result.Failure<Trainer, Error>(
                emailResult.Error);

        var phoneResult = Phone.Create(phone);

        if (phoneResult.IsFailure)
            return Result.Failure<Trainer, Error>(
                phoneResult.Error);

        TrainerNumber = trainerNumber;
        TrainerName = trainerName;
        Email = emailResult.Value;
        Phone = phoneResult.Value;
        IsActive = isActive;
        BranchId = branchId;

        return Result.Success<Trainer, Error>(this);
    }

    public Guid Id { get; private set; }

    public string TrainerNumber { get; private set; } = string.Empty;

    public string TrainerName { get; private set; } = string.Empty;

    public Email? Email { get; private set; }

    public Phone? Phone { get; private set; }

    public bool IsActive { get; private set; }

    public Guid BranchId { get; private set; }
}