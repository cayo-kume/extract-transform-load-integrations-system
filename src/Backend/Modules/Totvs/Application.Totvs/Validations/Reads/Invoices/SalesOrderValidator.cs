using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class SalesOrderValidator : AbstractValidator<SalesOrder>
    {
        public SalesOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.orderCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.orderCode));

            RuleFor(x => x.orderId)
                .MaximumLength(100)
                .WithMessage("O campo 'orderId' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.orderId));

            RuleFor(x => x.customerOrderCode)
                .MaximumLength(100)
                .WithMessage("O campo 'customerOrderCode' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerOrderCode));
        }
    }
}
