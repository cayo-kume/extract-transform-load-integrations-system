using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class CommissionedValidator : AbstractValidator<Commissioned>
    {
        public CommissionedValidator()
        {
            RuleFor(x => x.representativeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'representativeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.representativeName)
                .MaximumLength(60)
                .WithMessage("O campo 'representativeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeName));

            RuleFor(x => x.commissionPercentageBilling)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.commissionPercentageBilling));

            RuleFor(x => x.commissionPercentageReceipt)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.commissionPercentageReceipt));

            RuleFor(x => x.commissionedType)
                .MaximumLength(30)
                .WithMessage("O campo 'commissionedType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.commissionedType));
        }
    }
}
