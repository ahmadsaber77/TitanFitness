using FluentValidation;
using TitanFitness.Application.Members.Commands.CreateMember;

namespace TitanFitness.Application.Members.Validations;

public sealed class CreateMemberCommandValidator
    : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.MembershipNumber)
            .MaximumLength(10)
            .When(x => !string.IsNullOrWhiteSpace(x.MembershipNumber));

        RuleFor(x => x.Email)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.HomeBranchId)
            .NotEmpty();

        RuleFor(x => x.JoinedDate)
            .NotEmpty();
    }
}
