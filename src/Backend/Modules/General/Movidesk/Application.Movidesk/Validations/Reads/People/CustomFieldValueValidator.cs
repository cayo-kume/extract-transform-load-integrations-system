using Domain.Shared.Extensions;
using Domain.Movidesk.Dtos.Reads.People;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.People
{
    public class CustomFieldValueValidator : AbstractValidator<CustomFieldValue>
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

            RuleForEach(x => x.items)
                .SetValidator(new CustomFieldValueItemValidator());
        }
    }
}
