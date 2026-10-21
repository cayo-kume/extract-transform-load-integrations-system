using Domain.Shared.Extensions;
using Domain.Movidesk.Dtos.Reads.People;
using FluentValidation;

namespace Application.Movidesk.Validations.People
{
    public class CustomFieldValueItemValidator : AbstractValidator<CustomFieldValueItem>
    {
        public CustomFieldValueItemValidator()
        {
            // Cód. Ref. em texto (a documentação diz int, mas a API manda texto, como nos tickets)
            RuleFor(x => x.personId)
                .MaximumLength(64)
                .WithMessage("O campo 'personId' do item deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personId));

            RuleFor(x => x.clientId)
                .MaximumLength(64)
                .WithMessage("O campo 'clientId' do item deve ter no máximo 64 caracteres.")
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
