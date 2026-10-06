using Domain.Totvs.Dtos.SalesOrder;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class BatchItemValidator : AbstractValidator<BatchItem>
    {
        public BatchItemValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.quantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.quantity));
        }
    }
}
