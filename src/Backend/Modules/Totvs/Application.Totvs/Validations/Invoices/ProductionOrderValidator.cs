using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ProductionOrderValidator : AbstractValidator<ProductionOrder>
    {
        public ProductionOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.cycleCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cycleCode));

            RuleFor(x => x.orderCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.orderCode));

            RuleFor(x => x.orderRelationshipType)
                .MaximumLength(10)
                .WithMessage("O campo 'orderRelationshipType' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.orderRelationshipType));
        }
    }
}
