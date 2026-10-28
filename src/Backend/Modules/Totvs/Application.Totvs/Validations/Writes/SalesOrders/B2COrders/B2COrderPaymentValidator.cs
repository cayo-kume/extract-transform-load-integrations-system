using Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// pagamento do pedido B2C: tipo de documento, parcelas e valor total do pagamento; cartão com NSU, autorização, operadora e bandeira;
    /// SplitOmni com a empresa do financeiro
    /// </summary>
    public class B2COrderPaymentValidator : AbstractValidator<B2COrderPayment>
    {
        public B2COrderPaymentValidator()
        {
            RuleFor(x => x.documentType)
                .NotNull()
                .WithMessage("O campo 'documentType' é obrigatório.")
                .IsInEnum()
                .WithMessage("O campo 'documentType' só aceita os tipos do swagger (1 a 5, 13, 16, 18, 20, 23 a 27).");

            RuleFor(x => x.installment)
                .NotNull()
                .WithMessage("O campo 'installment' é obrigatório.")
                .InclusiveBetween(1, 999)
                .WithMessage("O campo 'installment' deve ter de 1 a 3 dígitos.");

            RuleFor(x => x.paymentValue)
                .NotNull()
                .WithMessage("O campo 'paymentValue' é obrigatório (total do pagamento, não da parcela).")
                .GreaterThan(0)
                .WithMessage("O campo 'paymentValue' deve ser maior que zero.")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'paymentValue' aceita até 2 casas decimais.");

            RuleFor(x => x)
                .Must(x => x.nsu is not null && !String.IsNullOrEmpty(x.authorizationCode) && !String.IsNullOrEmpty(x.creditCardOperator) && !String.IsNullOrEmpty(x.creditCardBrand))
                .WithName("nsu")
                .WithMessage("Pagamento com cartão leva 'nsu', 'authorizationCode', 'creditCardOperator' e 'creditCardBrand'.")
                .When(x => x.documentType is B2CPaymentDocumentType.CreditCard or B2CPaymentDocumentType.DebitCard);

            RuleFor(x => x)
                .Must(x => x.paymentBranch is not null || !String.IsNullOrEmpty(x.paymentBranchCnpj))
                .WithName("paymentBranch")
                .WithMessage("Com o paymentType SplitOmni, informe a empresa do financeiro ('paymentBranch' ou 'paymentBranchCnpj').")
                .When(x => x.paymentType == B2CPaymentType.SplitOmni);

            RuleFor(x => x)
                .Must(x => x.paymentBranch is null || String.IsNullOrEmpty(x.paymentBranchCnpj))
                .WithName("paymentBranch")
                .WithMessage("Informe a empresa do financeiro por 'paymentBranch' ou 'paymentBranchCnpj', não os dois.");

            RuleFor(x => x.paymentType).IsInEnum().WithMessage("O campo 'paymentType' só aceita de 0 a 3.");
            RuleFor(x => x.authorizationCode).MaximumLength(20).WithMessage("O campo 'authorizationCode' aceita até 20 caracteres.");
            RuleFor(x => x.creditCardOperator).MaximumLength(60).WithMessage("O campo 'creditCardOperator' aceita até 60 caracteres.");
            RuleFor(x => x.creditCardBrand).MaximumLength(60).WithMessage("O campo 'creditCardBrand' aceita até 60 caracteres.");
            RuleFor(x => x.alphanumericNsu).MaximumLength(100).WithMessage("O campo 'alphanumericNsu' aceita até 100 caracteres.");
        }
    }
}
