using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ConservationInstructionsItemValidator : AbstractValidator<ConservationInstructionsItem>
    {
        public ConservationInstructionsItemValidator()
        {
            RuleFor(x => x.order)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.order));

            RuleFor(x => x.item)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.item));
        }
    }
}
