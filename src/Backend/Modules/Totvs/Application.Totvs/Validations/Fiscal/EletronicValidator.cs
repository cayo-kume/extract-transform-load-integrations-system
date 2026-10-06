using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class EletronicValidator : AbstractValidator<Eletronic>
    {
        public EletronicValidator()
        {
            RuleFor(x => x.accessKey)
                .MaximumLength(44)
                .WithMessage("O campo 'accessKey' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.electronicInvoiceStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'electronicInvoiceStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.electronicInvoiceStatus));

            RuleFor(x => x.receipt)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.receipt));

            RuleFor(x => x.receivementDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.receivementDate));

            RuleFor(x => x.disableProtocol)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.disableProtocol));

            RuleFor(x => x.disableDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.disableDate));
        }
    }
}
