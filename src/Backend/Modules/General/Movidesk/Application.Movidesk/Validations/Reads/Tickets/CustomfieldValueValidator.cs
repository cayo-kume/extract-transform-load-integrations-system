using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Tickets
{
    public class CustomFieldValueValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.CustomFieldValue>
    {
        public CustomFieldValueValidator()
        {
            RuleFor(x => x.customFieldId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customFieldId));

            RuleFor(x => x.customFieldRuleId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customFieldRuleId));

            RuleFor(x => x.line)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.line));

            // value é nvarchar(max): sem limite de tamanho

            RuleForEach(x => x.items)
                .SetValidator(new CustomFieldValueItemValidator());
        }
    }
}
