using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class BatchItemValidator : AbstractValidator<BatchItem>
    {
        public BatchItemValidator()
        {
            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));
        }
    }
}
