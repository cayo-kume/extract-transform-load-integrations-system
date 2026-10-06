using Domain.Totvs.Dtos.Person.LegalEntity;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class SocialNetworkValidator : AbstractValidator<Socialnetwork>
    {
        public SocialNetworkValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.typeName)
                .MaximumLength(60)
                .WithMessage("O campo 'typeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.address)
                .MaximumLength(250)
                .WithMessage("O campo 'address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));
        }
    }
}
