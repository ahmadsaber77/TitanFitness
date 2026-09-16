using CSharpFunctionalExtensions;
using System.Numerics;
using TitanFitness.Domain.Common;
using TitanFitness.Domain.Enums;
using TitanFitness.Domain.ValueObjects;

namespace TitanFitness.Domain.Entities;

public class Membership : IAggregateRoot
{

    private Membership()
    { 
    }


    private Membership(
          Guid id,
          Guid memberId,
          Guid planId,
          DateTime purchaseDate,
          DateOnly startDate,
          DateOnly endDate,
          MembershipStatus status,
          AgreedTerms agreedTerms)
    {
        Id = id;
        MemberId = memberId;
        PlanId = planId;
        PurchaseDate = purchaseDate;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        AgreedTerms = agreedTerms;
    }

    public static Result<Membership, Error> Create(
        Guid memberId,
        Guid planId,
        DateTime purchaseDate,
        DateOnly startDate,
        AgreedTerms agreedTerms)
    {
        if (memberId == Guid.Empty)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Member is required."));

        if (planId == Guid.Empty)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Plan is required."));

        if (purchaseDate == default)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Purchase date is required."));

        if (startDate == default)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Start date is required."));

        if (agreedTerms is null)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Agreed terms are required."));

        var endDate = startDate.AddMonths(
            agreedTerms.DurationInMonths);

        var today = DateOnly.FromDateTime(
            purchaseDate);

        var status = startDate > today ? MembershipStatus.Pending : MembershipStatus.Active;

        var membership = new Membership(
            Guid.NewGuid(),
            memberId,
            planId,
            purchaseDate,
            startDate,
            endDate,
            status,
            agreedTerms);

        return Result.Success<Membership, Error>(
            membership);
    }


    public Result<Membership, Error> Cancel()
    {
        if (Status == MembershipStatus.Expired)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "expired membership cannot be cancelled"));

        if (Status == MembershipStatus.Cancelled)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    " membership is already cancelled"));
        Status = MembershipStatus.Cancelled;

        return Result.Success<Membership, Error>(this);

    }



    public Result<Freeze, Error> RequestFreeze(
        DateOnly startDate,
        int durationInMonths,
        string reason,
        string? additionalNotes)
    { 

    if (Status != MembershipStatus.Active)
    return Result.Failure<Freeze,Error>(
        Error.Conflict<Freeze>(
         "Only active membership can be frozen."));

        if (durationInMonths is < 1 or > 3)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Freeze duration must be 1, 2, or 3 months."));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (startDate < today)
            return Result.Failure<Freeze, Error>(
            Error.Validation<Freeze>(
            "Freeze start date cannot be in the past."));

        var freezeEndDate = startDate.AddMonths(durationInMonths);
        if (freezeEndDate > EndDate)
               return Result.Failure<Freeze, Error>(
               Error.Validation<Freeze>(
               "Freeze cannot extend beyond membership end date."));

        if (_freezes.Count >= AgreedTerms.MaxNumberOfFreezes)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Maximum number of freeze days have been exceeded."));

        var freezeDays = freezeEndDate.DayNumber - startDate.DayNumber;

        var totalFreezeDays =
    _freezes.Sum(x => x.EndDate.DayNumber - x.StartDate.DayNumber)
    + freezeDays;

        if (totalFreezeDays > AgreedTerms.MaxFreezeDays)
            return Result.Failure<Freeze, Error>(
                Error.Conflict<Freeze>(
                    "Maximum freeze days have been exceeded."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Freeze reason is required."));

        if (additionalNotes is not null && additionalNotes.Length > 200)
            return Result.Failure<Freeze, Error>(
                Error.Validation<Freeze>(
                    "Additional notes cannot exceed 200 characters."));


        var freezeResult = Freeze.Create(
                    Id,
                    startDate,
                    freezeEndDate,
                    durationInMonths,
                    reason,
                    additionalNotes,
                    DateTime.UtcNow);

            if (freezeResult.IsFailure)
                return Result.Failure<Freeze, Error>(
                    freezeResult.Error);

            var freeze = freezeResult.Value;
            _freezes.Add(freeze);
            Status = MembershipStatus.Frozen;
            EndDate = EndDate.AddDays(freezeDays);
            return Result.Success<Freeze, Error>(freeze); 
    }

    public Result<Membership, Error> Renew(Plan plan)
    {
        if (Status != MembershipStatus.Expired)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Only expired membership can be renewed."));

        if (plan is null)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Plan is required."));

        if (!plan.IsPublished)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Cannot renew using an unpublished plan."));

        var purchaseDate = DateTime.UtcNow;

        var newStartDate = DateOnly.FromDateTime(
            purchaseDate);

        var agreedTermsResult = AgreedTerms.Create(
            plan.Price,
            plan.DurationInMonths,
            plan.MaxFreezeDays,
            plan.MaxNumberOfFreezes,
            plan.GuestPassQuota,
            plan.AccessScope);

        if (agreedTermsResult.IsFailure)
            return Result.Failure<Membership, Error>(
                agreedTermsResult.Error);

        var newMembershipResult = Membership.Create(
            MemberId,
            plan.Id,
            purchaseDate,
            newStartDate,
            agreedTermsResult.Value);

        if (newMembershipResult.IsFailure)
            return Result.Failure<Membership, Error>(
                newMembershipResult.Error);

        return Result.Success<Membership, Error>(
            newMembershipResult.Value);
    }

    public Result<Membership, Error> EndFreeze(DateOnly resumedOn)
    {
        if (Status != MembershipStatus.Frozen)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Membership is not currently frozen."));

        var activeFreeze = _freezes.LastOrDefault();

        if (activeFreeze is null)
            return Result.Failure<Membership, Error>(
             Error.Failure(
             "No freeze record found to resume from."));

        if (resumedOn == default)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Resume date is required."));

        if (resumedOn < activeFreeze.StartDate)
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Resume date cannot be before freeze start date."));

        var frozenDays =
            resumedOn.DayNumber - activeFreeze.StartDate.DayNumber;

        Status = MembershipStatus.Active;

        return Result.Success<Membership, Error>(this);
    }


    public Result<GuestPass, Error> IssueGuestPass(DateTime issuedOn)
    {
        if (Status != MembershipStatus.Active)
            return Result.Failure<GuestPass, Error>(
                Error.Conflict<Membership>(
                    "Only active membership can issue a guest pass."));

        if (_guestPasses.Count >= AgreedTerms.GuestPassQuota)
            return Result.Failure<GuestPass, Error>(
                Error.Conflict<Membership>(
                    "Guest pass quota has been used up."));


        if (issuedOn == default)
            return Result.Failure<GuestPass, Error>(
                Error.Validation<GuestPass>(
                    "Issue date is required."));

        var passResult = GuestPass.Create(
            Id,
            issuedOn);

        if (passResult.IsFailure)
            return Result.Failure<GuestPass, Error>(
                passResult.Error);

        var guestPass = passResult.Value;

        _guestPasses.Add(guestPass);

        return Result.Success<GuestPass, Error>(
            guestPass);
    }


    public Result<Membership, Error> Activate()
    {
        if (Status != MembershipStatus.Pending)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Only pending membership can be activated."));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (StartDate > today)
        {
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Membership cannot be activated before its start date."));
        }

        Status = MembershipStatus.Active;

        return Result.Success<Membership, Error>(this);
    }


    public Result<Membership, Error> Expire()
    {
        if (Status != MembershipStatus.Active)
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Only active membership can be expired."));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (EndDate > today)
        {
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Membership cannot be expired before its end date."));
        }

        Status = MembershipStatus.Expired;

        return Result.Success<Membership, Error>(this);
    }




    public Result<Membership, Error> ChangePlanAtRenewal(Plan plan)
    {
        if (Status != MembershipStatus.Expired || Status != MembershipStatus.Active)
        {
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Only expired  and activated membership can change plan at renewal."));
        }

        if (plan is null)
        {
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Plan is required."));
        }

        if (!plan.IsPublished)
        {
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Cannot change plan using an unpublished plan."));
        }

        var agreedTermsResult = AgreedTerms.Create(
            plan.Price,
            plan.DurationInMonths,
            plan.MaxFreezeDays,
            plan.MaxNumberOfFreezes,
            plan.GuestPassQuota,
            plan.AccessScope);

        if (agreedTermsResult.IsFailure)
        {
            return Result.Failure<Membership, Error>(
                agreedTermsResult.Error);
        }

        var purchaseDate = DateTime.UtcNow;
        var newStartDate = DateOnly.FromDateTime(purchaseDate);

        var newMembershipResult = Membership.Create(
            MemberId,
            plan.Id,
            purchaseDate,
            newStartDate,
            agreedTermsResult.Value);

        if (newMembershipResult.IsFailure)
        {
            return Result.Failure<Membership, Error>(
                newMembershipResult.Error);
        }

        return Result.Success<Membership, Error>(
            newMembershipResult.Value);
    }




    public Result<Membership, Error> ChangePlanImmediately(Plan plan)
    {
        if (Status != MembershipStatus.Active)
        {
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Only active membership can change plan immediately."));
        }

        if (plan is null)
        {
            return Result.Failure<Membership, Error>(
                Error.Validation<Membership>(
                    "Plan is required."));
        }

        if (!plan.IsPublished)
        {
            return Result.Failure<Membership, Error>(
                Error.Conflict<Membership>(
                    "Cannot change plan using an unpublished plan."));
        }

        var agreedTermsResult = AgreedTerms.Create(
            plan.Price,
            plan.DurationInMonths,
            plan.MaxFreezeDays,
            plan.MaxNumberOfFreezes,
            plan.GuestPassQuota,
            plan.AccessScope);

        if (agreedTermsResult.IsFailure)
        {
            return Result.Failure<Membership, Error>(
                agreedTermsResult.Error);
        }

        var cancelResult = Cancel();

        if (cancelResult.IsFailure)
        {
            return Result.Failure<Membership, Error>(
                cancelResult.Error);
        }

        var purchaseDate = DateTime.UtcNow;
        var newStartDate = DateOnly.FromDateTime(purchaseDate);

        var newMembershipResult = Membership.Create(
            MemberId,
            plan.Id,
            purchaseDate,
            newStartDate,
            agreedTermsResult.Value);

        if (newMembershipResult.IsFailure)
        {
            return Result.Failure<Membership, Error>(
                newMembershipResult.Error);
        }

        return Result.Success<Membership, Error>(
            newMembershipResult.Value);
    }



    public Guid Id { get; private set; }

    public Guid MemberId { get; private set; }

    public Guid PlanId { get; private set; }

    public DateTime PurchaseDate { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public MembershipStatus Status { get; private set; }

    public AgreedTerms AgreedTerms { get; private set; } = null!;


    private readonly List<Freeze> _freezes = new();
    public IReadOnlyCollection<Freeze> Freezes => _freezes;


    private readonly List<GuestPass> _guestPasses = new();
    public IReadOnlyCollection<GuestPass> GuestPasses => _guestPasses;
}

