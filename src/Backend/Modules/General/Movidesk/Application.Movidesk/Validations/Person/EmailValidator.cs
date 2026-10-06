using Domain.Core.Extensions;
using Domain.Movidesk.Dtos.Person;
using FluentValidation;

namespace Application.Movidesk.Validations.Person
{
    public class EmailValidator : AbstractValidator<Email>
    {
        public EmailValidator()
        {
            RuleFor(x => x.emailType)
                .MaximumLength(128)
                .WithMessage("O campo 'emailType' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.emailType));

            RuleFor(x => x.email)
                .MaximumLength(128)
                .WithMessage("O campo 'email' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));
        }
    }
}
