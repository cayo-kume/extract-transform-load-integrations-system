using Domain.Shared.Extensions;
using Domain.Movidesk.Dtos.Reads.People;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.People
{
    public class RelationshipValidator : AbstractValidator<Relationship>
    {
        public RelationshipValidator()
        {
            RuleFor(x => x.id)
                .MaximumLength(64)
                .WithMessage("O campo 'id' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(128)
                .WithMessage("O campo 'name' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.slaAgreement)
                .MaximumLength(128)
                .WithMessage("O campo 'slaAgreement' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.slaAgreement));

            RuleFor(x => x.forceChildrenToHaveSomeAgreement)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.forceChildrenToHaveSomeAgreement));

            RuleFor(x => x.allowAllServices)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.allowAllServices));

            RuleFor(x => x.includeInParents)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.includeInParents));

            RuleFor(x => x.loadChildOrganizations)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.loadChildOrganizations));
        }
    }
}
