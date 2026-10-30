using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Tickets
{
    public class CustomFieldValueItemValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.CustomFieldValueItem>
    {
        public CustomFieldValueItemValidator()
        {
            RuleFor(x => x.personId)
                .MaximumLength(64)
                .WithMessage("O campo 'personId' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personId));

            RuleFor(x => x.clientId)
                .MaximumLength(64)
                .WithMessage("O campo 'clientId' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.clientId));

            RuleFor(x => x.team)
                .MaximumLength(128)
                .WithMessage("O campo 'team' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.team));

            RuleFor(x => x.customFieldItem)
                .MaximumLength(256)
                .WithMessage("O campo 'customFieldItem' deve ter no máximo 256 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customFieldItem));

            RuleFor(x => x.fileName)
                .MaximumLength(255)
                .WithMessage("O campo 'fileName' deve ter no máximo 255 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fileName));
        }
    }
}
