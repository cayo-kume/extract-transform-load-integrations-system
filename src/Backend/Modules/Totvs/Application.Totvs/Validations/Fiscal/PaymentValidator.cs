using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.documentNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documentNumber));

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
                .MaximumLength(60)
                .WithMessage("O campo 'documentType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documentType));

            RuleFor(x => x.installment)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.installment));

            RuleFor(x => x.bearerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bearerCode));

            RuleFor(x => x.bearerName)
                .MaximumLength(60)
                .WithMessage("O campo 'bearerName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bearerName));

            RuleFor(x => x.cardInformation)
                .SetValidator(new CardInformationValidator());

            RuleFor(x => x.digitalWalletInformation)
                .SetValidator(new DigitalWalletInformationValidator());
        }
    }
}
