using Domain.Totvs.Dtos.Writes.Products.Batches;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Batches
{
    /// <summary>
    /// valida o corpo dos pedidos preferenciais do item de lote no controller (corpo inválido volta 400): o item por lote + sequência ou pela
    /// barra, um ou outro; cada pedido pela loja e pelo número ou pelo orderId, sem repetição. A lista vazia (que só apaga) é tratada no
    /// controller, com ?clear=true
    /// </summary>
    public class BatchPreferentialSalesOrderValidator : AbstractValidator<BatchPreferentialSalesOrder>
    {
        public BatchPreferentialSalesOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x)
                .Must(x => (x.batchCode is not null && x.sequence is not null && String.IsNullOrEmpty(x.batchItemBarCode))
                    || (x.batchCode is null && x.sequence is null && !String.IsNullOrEmpty(x.batchItemBarCode)))
                .WithName("batchCode")
                .WithMessage("Informe o item por 'batchCode' + 'sequence' ou por 'batchItemBarCode', um ou outro.");

            RuleFor(x => x.batchItemBarCode)
                .MaximumLength(40)
                .WithMessage("O campo 'batchItemBarCode' aceita até 40 caracteres.");

            RuleFor(x => x.preferentialSalesOrder)
                .Must(l => l!.Where(o => o is not null).GroupBy(o => (o.branchCode, o.orderCode, o.orderId)).All(g => g.Count() == 1))
                .WithMessage("O mesmo pedido aparece mais de uma vez em 'preferentialSalesOrder'.")
                .When(x => x.preferentialSalesOrder is not null);

            RuleForEach(x => x.preferentialSalesOrder)
                .ChildRules(o =>
                {
                    o.RuleFor(x => x.branchCode)
                        .NotNull()
                        .WithMessage("O campo 'branchCode' do pedido é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'branchCode' do pedido deve ter de 1 a 4 dígitos.");

                    o.RuleFor(x => x)
                        .Must(x => (x.orderCode is null) != String.IsNullOrEmpty(x.orderId))
                        .WithName("orderCode")
                        .WithMessage("Informe exatamente um entre 'orderCode' e 'orderId' no pedido.");

                    o.RuleFor(x => x.orderCode)
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.")
                        .When(x => x.orderCode is not null);

                    o.RuleFor(x => x.orderId)
                        .MaximumLength(100)
                        .WithMessage("O campo 'orderId' aceita até 100 caracteres.");
                })
                .When(x => x.preferentialSalesOrder is not null);
        }
    }
}
