using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class CreatedbyValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Createdby>
    {
        public CreatedbyValidator()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.personType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personType));

            RuleFor(x => x.profileType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.profileType));

            RuleFor(x => x.businessName).MaximumLength(60)
                .When(x => !string.IsNullOrEmpty(x.businessName));

            RuleFor(x => x.email)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.phone).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.phone));

            RuleFor(x => x.address).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.complement).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.cep).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.city).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.city));

            RuleFor(x => x.bairro).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.bairro));

            RuleFor(x => x.number).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.number));

            RuleFor(x => x.reference).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.reference));
        }
    }
}
