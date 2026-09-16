using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;

namespace TitanFitness.Domain.Entities;

public class Studio :IEntity
{

    private Studio()
    {
    }

    private Studio(
        Guid id,
        string name,
        Guid branchId,
        int capacity)
    {
        Id = id;
        Name = name;
        BranchId = branchId;
        Capacity = capacity;
    }

    internal static Result<Studio, Error> Create(
        string name,
        Guid branchId,
        int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>("Studio name is required."));

        if (name.Length > 50)
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>(
                    "Studio name cannot exceed 50 characters."));

        if (branchId == Guid.Empty)
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>("Branch is required."));

        if (capacity <= 0)
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>(
                    "Studio capacity must be greater than zero."));

        var studio = new Studio(
            Guid.NewGuid(),
            name,
            branchId,
            capacity);

        return Result.Success<Studio, Error>(studio);
    }


    internal Result<Studio, Error> Update(
    string name,
    int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>(
                    "Studio name is required."));

        if (name.Length > 50)
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>(
                    "Studio name cannot exceed 50 characters."));

        if (capacity <= 0)
            return Result.Failure<Studio, Error>(
                Error.Validation<Studio>(
                    "Studio capacity must be greater than zero."));

        Name = name;
        Capacity = capacity;

        return Result.Success<Studio, Error>(this);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Guid BranchId { get; private set; }

    public int Capacity { get; private set; }
}
