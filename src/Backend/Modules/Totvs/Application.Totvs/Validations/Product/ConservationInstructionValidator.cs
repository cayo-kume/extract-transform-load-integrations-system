using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ConservationInstructionValidator : AbstractValidator<ConservationInstruction>
    {
        public ConservationInstructionValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x._default)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x._default));

            RuleFor(x => x.grouperCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.grouperCode));

            RuleFor(x => x.description)
                .MaximumLength(300)
                .WithMessage("O campo 'description' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleForEach(x => x.items)
                .SetValidator(new ConservationInstructionsItemValidator());
        }
    }
}
