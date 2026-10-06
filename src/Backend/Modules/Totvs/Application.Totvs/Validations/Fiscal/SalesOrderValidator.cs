using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class SalesOrderValidator : AbstractValidator<Domain.Totvs.Dtos.Fiscal.SalesOrder>
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
                .MaximumLength(60)
                .WithMessage("O campo 'orderId' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.orderId));

            RuleFor(x => x.customerOrderCode)
                .MaximumLength(60)
                .WithMessage("O campo 'customerOrderCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerOrderCode));
        }
    }
}
