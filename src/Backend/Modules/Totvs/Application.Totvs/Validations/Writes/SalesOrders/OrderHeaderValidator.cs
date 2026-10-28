using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo da capa do pedido de venda no controller (corpo inválido volta 400): a chave e pelo menos um campo a mudar
    /// </summary>
    public class OrderHeaderValidator : AbstractValidator<OrderHeader>
    {
        public OrderHeaderValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x)
                .Must(x => x.averagePaymentPeriod is not null || x.operationCode is not null || x.priceTableCode is not null || x.priorityCode is not null)
                .WithName("averagePaymentPeriod")
                .WithMessage("Informe pelo menos um entre 'averagePaymentPeriod', 'operationCode', 'priceTableCode' e 'priorityCode'.");

            RuleFor(x => x.averagePaymentPeriod)
                .InclusiveBetween(0, 999_999)
                .WithMessage("O campo 'averagePaymentPeriod' deve ter até 6 dígitos.")
                .When(x => x.averagePaymentPeriod is not null);

            RuleFor(x => x.operationCode)
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'operationCode' deve ter de 1 a 4 dígitos.")
                .When(x => x.operationCode is not null);

            RuleFor(x => x.priceTableCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'priceTableCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.priceTableCode is not null);

            RuleFor(x => x.priorityCode)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'priorityCode' deve ter até 2 dígitos.")
                .When(x => x.priorityCode is not null);
        }
    }
}
