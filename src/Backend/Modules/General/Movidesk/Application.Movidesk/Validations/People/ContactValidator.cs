using Domain.Shared.Extensions;
using Domain.Movidesk.Dtos.Reads.People;
using FluentValidation;

namespace Application.Movidesk.Validations.People
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.contactType)
                .MaximumLength(128)
                .WithMessage("O campo 'contactType' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contactType));

            RuleFor(x => x.contact)
                .MaximumLength(128)
                .WithMessage("O campo 'contact' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contact));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));
        }
    }
}
