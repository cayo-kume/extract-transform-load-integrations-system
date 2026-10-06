using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class CommissionedValidator : AbstractValidator<Commissioned>
    {
        public CommissionedValidator()
        {
            RuleFor(x => x.representativeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.representativeCode));

            RuleFor(x => x.commissionPercentageReceipt)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.commissionPercentageReceipt));

            RuleFor(x => x.commissionPercentageBilling)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.commissionPercentageBilling));

            RuleFor(x => x.representativeName)
                .MaximumLength(250)
                .WithMessage("O campo 'representativeName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeName));

            RuleFor(x => x.commissionedType)
                .MaximumLength(60)
                .WithMessage("O campo 'commissionedType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.commissionedType));
        }
    }
}
