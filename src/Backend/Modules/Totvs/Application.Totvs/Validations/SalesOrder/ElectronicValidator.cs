using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class ElectronicValidator : AbstractValidator<Electronic>
    {
        public ElectronicValidator()
        {
            RuleFor(x => x.receipt)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.receipt));

            RuleFor(x => x.receivementDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.receivementDate));

            RuleFor(x => x.accessKey)
                .MaximumLength(44)
                .WithMessage("O campo 'accessKey' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.electronicInvoiceStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'electronicInvoiceStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.electronicInvoiceStatus));
        }
    }
}
