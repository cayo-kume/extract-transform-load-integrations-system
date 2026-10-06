using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Stone.Validations
{
    public class StoneItemValidator : AbstractValidator<Domain.Stone.Dtos.Item>
    {
        public StoneItemValidator()
        {
            RuleFor(x => x.depth).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.depth));
            RuleFor(x => x.width).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.width));
            RuleFor(x => x.height).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.height));
            RuleFor(x => x.weight).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.weight));
            RuleFor(x => x.quantity).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.quantity));
            RuleFor(x => x.unitPrice).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.unitPrice));
            RuleFor(x => x.invoiceTotalValue).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.invoiceTotalValue));
            RuleFor(x => x.code).MaximumLength(50).WithMessage("O campo 'code' deve ter no máximo 50 caractere.").When(x => !string.IsNullOrEmpty(x.code));
            RuleFor(x => x.invoiceKey).MaximumLength(44).WithMessage("O campo 'invoiceKey' deve ter no máximo 44 caractere.").When(x => !string.IsNullOrEmpty(x.invoiceKey));
            RuleFor(x => x.description).MaximumLength(255).WithMessage("O campo 'description' deve ter no máximo 255 caractere.").When(x => !string.IsNullOrEmpty(x.description));
            RuleFor(x => x.invoiceNumber).MaximumLength(50).WithMessage("O campo 'invoiceNumber' deve ter no máximo 50 caractere.").When(x => !string.IsNullOrEmpty(x.invoiceNumber));
        }
    }
}
