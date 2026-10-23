using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class ElectronicValidator : AbstractValidator<Electronic>
    {
        public ElectronicValidator()
        {
            RuleFor(x => x.accessKey)
                .MaximumLength(200)
                .WithMessage("O campo 'accessKey' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.electronicInvoiceStatus)
                .MaximumLength(20)
                .WithMessage("O campo 'electronicInvoiceStatus' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.electronicInvoiceStatus));

            RuleFor(x => x.receipt)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.receipt));

            RuleFor(x => x.receivementDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.receivementDate));
        }
    }
}
