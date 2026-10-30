using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.documentNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'documentNumber' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.installment)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'installment' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.expirationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.expirationDate));

            RuleFor(x => x.paymentValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.paymentValue));

            RuleFor(x => x.documentTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documentTypeCode));

            RuleFor(x => x.documentType)
                .MaximumLength(30)
                .WithMessage("O campo 'documentType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documentType));

            RuleFor(x => x.bearerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bearerCode));

            RuleFor(x => x.bearerName)
                .MaximumLength(40)
                .WithMessage("O campo 'bearerName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bearerName));

            RuleFor(x => x.cardInformation!)
                .SetValidator(new CardInformationValidator())
                .When(x => x.cardInformation is not null);

            RuleFor(x => x.digitalWalletInformation!)
                .SetValidator(new DigitalWalletInformationValidator())
                .When(x => x.digitalWalletInformation is not null);
        }
    }
}
