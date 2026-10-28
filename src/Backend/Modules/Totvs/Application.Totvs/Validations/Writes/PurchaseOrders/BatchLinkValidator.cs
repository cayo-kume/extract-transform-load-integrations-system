using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// valida o corpo do vínculo de item de lote com pedido de compra no controller (corpo inválido volta 400): a loja do lote, o item por lote +
    /// sequência ou pela barra, um ou outro, o pedido e a nota pela chave de acesso ou pela série e número
    /// </summary>
    public class BatchLinkValidator : AbstractValidator<BatchLink>
    {
        public BatchLinkValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório (empresa do lote).")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x)
                .Must(x => (x.batchCode is not null && x.sequence is not null && String.IsNullOrEmpty(x.batchItemBarCode))
                    || (x.batchCode is null && x.sequence is null && !String.IsNullOrEmpty(x.batchItemBarCode)))
                .WithName("batchCode")
                .WithMessage("Informe o item por 'batchCode' + 'sequence' ou por 'batchItemBarCode', um ou outro.");

            RuleFor(x => x.batchCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'batchCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.batchCode is not null);

            RuleFor(x => x.sequence)
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'sequence' deve ter de 1 a 4 dígitos.")
                .When(x => x.sequence is not null);

            RuleFor(x => x.batchItemBarCode).MaximumLength(40).WithMessage("O campo 'batchItemBarCode' aceita até 40 caracteres.");

            RuleFor(x => x.orderCode)
                .NotNull()
                .WithMessage("O campo 'orderCode' é obrigatório (o pedido do vínculo).")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.invoice!)
                .ChildRules(i =>
                {
                    i.RuleFor(x => x)
                        .Must(x => !String.IsNullOrEmpty(x.accessKey) != (!String.IsNullOrEmpty(x.serialCode) && x.invoiceCode is not null))
                        .WithName("invoice.accessKey")
                        .WithMessage("Informe a nota por 'accessKey' ou por 'serialCode' + 'invoiceCode', um ou outro.");

                    i.RuleFor(x => x.accessKey)
                        .Matches(@"^\d{44}$")
                        .WithMessage("O campo 'invoice.accessKey' deve ter 44 dígitos.")
                        .When(x => !String.IsNullOrEmpty(x.accessKey));

                    i.RuleFor(x => x.serialCode).MaximumLength(3).WithMessage("O campo 'invoice.serialCode' aceita até 3 caracteres.");

                    i.RuleFor(x => x.invoiceCode)
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'invoice.invoiceCode' deve ter de 1 a 9 dígitos.")
                        .When(x => x.invoiceCode is not null);
                })
                .When(x => x.invoice is not null);
        }
    }
}
