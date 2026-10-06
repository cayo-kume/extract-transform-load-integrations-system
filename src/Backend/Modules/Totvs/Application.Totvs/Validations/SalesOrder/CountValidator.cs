using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class CountValidator : AbstractValidator<Count>
    {
        public CountValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.packagingTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.packagingTypeCode));

            RuleFor(x => x.separator)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.separator));

            RuleFor(x => x.description)
                .MaximumLength(250)
                .WithMessage("O campo 'description' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.statusCount)
                .MaximumLength(60)
                .WithMessage("O campo 'statusCount' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.statusCount));

            RuleFor(x => x.packagingTypeDescription)
                .MaximumLength(250)
                .WithMessage("O campo 'packagingTypeDescription' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.packagingTypeDescription));

            RuleFor(x => x.separatorName)
                .MaximumLength(250)
                .WithMessage("O campo 'separatorName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.separatorName));
        }
    }
}
