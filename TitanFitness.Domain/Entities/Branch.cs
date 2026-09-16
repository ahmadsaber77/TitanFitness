using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;

namespace TitanFitness.Domain.Entities;

public class Branch : IAggregateRoot
{
    private Branch()
    {

    }

    private Branch(
       Guid id,
       string name,
       string address,
       TimeOnly openingTime,
       TimeOnly closingTime)
    {
        Id = id;
        Name = name;
        Address = address;
        OpeningTime = openingTime;
        ClosingTime = closingTime;
    }

    public static Result<Branch, Error> Create(
        string name,
        string address,
        TimeOnly openingTime,
        TimeOnly closingTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>("Branch name is required."));

        if (name.Length > 50)
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>("Branch name cannot exceed 50 characters."));

        if (address is not null && address.Length > 200)
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>(
                    "Address cannot exceed 200 characters."));

        if (openingTime >= closingTime)
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>(
                    "Opening time must be earlier than closing time."));

        var branch = new Branch(
            Guid.NewGuid(),
            name,
            address!,
            openingTime,
            closingTime);

        return Result.Success<Branch, Error>(branch);
    }


    public Result<Branch, Error> Update(
        string name,
        string? address,
        TimeOnly openingTime,
        TimeOnly closingTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>(
                    "Branch name is required."));

        if (name.Length > 50)
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>(
                    "Branch name cannot exceed 50 characters."));

        if (address is not null && address.Length > 200)
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>(
                    "Address cannot exceed 200 characters."));

        if (openingTime >= closingTime)
            return Result.Failure<Branch, Error>(
                Error.Validation<Branch>(
                    "Opening time must be earlier than closing time."));

        Name = name;
        Address = address;
        OpeningTime = openingTime;
        ClosingTime = closingTime;

        return Result.Success<Branch, Error>(this);
    }


    public Result<Studio, Error> AddStudio(
    string name,
    int capacity)
    {
        var studioResult = Studio.Create(
            name,
            Id,
            capacity);

        if (studioResult.IsFailure)
            return Result.Failure<Studio, Error>(
                studioResult.Error);

        var studio = studioResult.Value;

        _studios.Add(studio);

        return Result.Success<Studio, Error>(studio);
    }


    public Result<Studio, Error> UpdateStudio(
    Guid studioId,
    string name,
    int capacity)
    {
        var studio = _studios.Where(x => x.Id == studioId)
        .FirstOrDefault();

        if (studio is null)
            return Result.Failure<Studio, Error>(
                Error.NotFound<Studio>(studioId));

        return studio.Update(name, capacity);
    }

    public Guid Id {  get; private set; }

    public string Name {   get; private set; } = string.Empty;

    public string? Address {  get; private set; } 

    public TimeOnly OpeningTime {   get; private set; }

    public TimeOnly ClosingTime {   get; private set; }


    private readonly List<Studio> _studios = new();
    public IReadOnlyCollection<Studio> Studios => _studios;
}
