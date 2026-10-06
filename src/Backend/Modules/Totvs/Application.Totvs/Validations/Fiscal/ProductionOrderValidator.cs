using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class ProductionOrderValidator : AbstractValidator<ProductionOrder>
    {
        public ProductionOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.orderCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.orderCode));

            RuleFor(x => x.cycleCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cycleCode));

            RuleFor(x => x.orderRelationshipType)
                .MaximumLength(60)
                .WithMessage("O campo 'orderRelationshipType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.orderRelationshipType));
        }
    }
}
