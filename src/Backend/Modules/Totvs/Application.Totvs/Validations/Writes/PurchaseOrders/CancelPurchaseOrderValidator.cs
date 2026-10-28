using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// valida o corpo do cancelamento do pedido de compra no controller (corpo inválido volta 400): loja, pedido e o fornecedor esperado por
    /// código ou por CPF/CNPJ (DP3)
    /// </summary>
    public class CancelPurchaseOrderValidator : AbstractValidator<CancelPurchaseOrder>
    {
        public CancelPurchaseOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.orderCode)
                .NotNull()
                .WithMessage("O campo 'orderCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x)
                .Must(x => (x.supplierCode is null) != String.IsNullOrEmpty(x.supplierCpfCnpj))
                .WithName("supplierCode")
                .WithMessage("Informe o fornecedor esperado no pedido por exatamente um entre 'supplierCode' e 'supplierCpfCnpj' (conferido antes de cancelar).");

            RuleFor(x => x.supplierCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'supplierCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.supplierCpfCnpj));
        }
    }
}
