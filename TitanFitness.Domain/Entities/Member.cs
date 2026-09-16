using CSharpFunctionalExtensions;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Domain.Entities;
public class Member : IAggregateRoot
{
    private Member()
    {
    }

    private Member(
        Guid id,
        MembershipNumber membershipNumber,
        string fullName,
        DateOnly joinedDate,
        Guid homeBranchId,
        Email? email,
        Phone? phone,
        string? address,
        byte[]? photo)
    {
        Id = id;
        MembershipNumber = membershipNumber;
        FullName = fullName;
        JoinedDate = joinedDate;
        HomeBranchId = homeBranchId;
        Email = email;
        Phone = phone;
        Address = address;
        Photo = photo;
    }

    public static Result<Member, Error> Create(
        MembershipNumber membershipNumber,
        string fullName,
        DateOnly joinedDate,
        Guid homeBranchId,
        Email? email,
        string? phone,
        string? address,
        byte[]? photo)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Full name cannot be empty."));

        if (fullName.Length > 100)
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Full name cannot exceed 100 characters."));

        if (homeBranchId == Guid.Empty)
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Branch is required."));

        if (address is not null && address.Length > 200)
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Address cannot exceed 200 characters."));

        var phoneResult = Phone.Create(phone);

        if (phoneResult.IsFailure)
            return Result.Failure<Member, Error>(
                phoneResult.Error);

        var member = new Member(
            Guid.NewGuid(),
            membershipNumber,
            fullName,
            joinedDate,
            homeBranchId,
            email,
            phoneResult.Value,
            address,
            photo);

        return Result.Success<Member, Error>(member);
    }

    public Result<Member, Error> UpdateProfile(
        string fullName,
        DateOnly joinedDate,
        Guid homeBranchId,
        Email? email,
        string? phone,
        string? address,
        byte[]? photo)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Full name cannot be empty."));

        if (fullName.Length > 100)
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Full name cannot exceed 100 characters."));

        if (homeBranchId == Guid.Empty)
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Branch is required."));

        if (address is not null && address.Length > 200)
            return Result.Failure<Member, Error>(
                Error.Validation<Member>(
                    "Address cannot exceed 200 characters."));

        var phoneResult = Phone.Create(phone);

        if (phoneResult.IsFailure)
            return Result.Failure<Member, Error>(
                phoneResult.Error);

        FullName = fullName;
        JoinedDate = joinedDate;
        HomeBranchId = homeBranchId;
        Email = email;
        Phone = phoneResult.Value;
        Address = address;
        Photo = photo;

        return Result.Success<Member, Error>(this);
    }

    public Guid Id { get; private set; }

    public MembershipNumber MembershipNumber { get; private set; } = null!;

    public string FullName { get; private set; } = string.Empty;

    public DateOnly JoinedDate { get; private set; }

    public Guid HomeBranchId { get; private set; }

    public Email? Email { get; private set; }

    public Phone? Phone { get; private set; }

    public string? Address { get; private set; }

    public byte[]? Photo { get; private set; }
}