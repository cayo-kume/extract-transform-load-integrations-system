using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class CustomfieldValueValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Customfieldvalue>
    {
        public CustomfieldValueValidator()
        {
            RuleFor(x => x.customFieldId)
                .MaximumLength(50)
                .WithMessage("O campo 'customFieldId' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customFieldId));

            RuleFor(x => x.customFieldRuleId)
                .MaximumLength(50)
                .WithMessage("O campo 'customFieldRuleId' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customFieldRuleId));

            RuleFor(x => x.line)
                .MaximumLength(50)
                .WithMessage("O campo 'line' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.line));

            RuleFor(x => x.value)
                .MaximumLength(50)
                .WithMessage("O campo 'value' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.value));

            RuleForEach(x => x.items)
                .SetValidator(new ItemValidator());
        }
    }
}
