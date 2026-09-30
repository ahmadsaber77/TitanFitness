using FluentValidation;
using TitanFitness.Application.Members.Commands.UpdateMember;

namespace TitanFitness.Application.Members.Contracts;

public sealed record UpdateMemberRequest(
    string FullName,
    string? Email,
    string? Phone,
    string? Address,
    DateOnly JoinedDate,
    Guid HomeBranchId,
    byte[]? Photo);



public sealed class UpdateMemberCommandValidator
    : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.JoinedDate)
            .NotEmpty();

        RuleFor(x => x.HomeBranchId)
            .NotEmpty();
    }
}