using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class WebDataValidator : AbstractValidator<WebData>
    {
        public WebDataValidator()
        {
            RuleFor(x => x.description)
                .MaximumLength(300)
                .WithMessage("O campo 'description' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.descriptive)
                .MaximumLength(300)
                .WithMessage("O campo 'descriptive' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descriptive));

            RuleFor(x => x.definition1)
                .MaximumLength(300)
                .WithMessage("O campo 'definition1' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.definition1));

            RuleFor(x => x.definition2)
                .MaximumLength(300)
                .WithMessage("O campo 'definition2' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.definition2));

            RuleFor(x => x.link)
                .MaximumLength(8000)
                .WithMessage("O campo 'link' deve ter no máximo 8000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.link));

            RuleFor(x => x.url)
                .MaximumLength(8000)
                .WithMessage("O campo 'url' deve ter no máximo 8000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url));

            RuleFor(x => x.status)
                .MaximumLength(60)
                .WithMessage("O campo 'status' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.weight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.weight));

            RuleFor(x => x.height)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.height));

            RuleFor(x => x.width)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.width));

            RuleFor(x => x.length)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.length));

            RuleFor(x => x.packWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packWeight));

            RuleFor(x => x.packHeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packHeight));

            RuleFor(x => x.packWidth)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packWidth));

            RuleFor(x => x.packLength)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packLength));

            RuleFor(x => x.productPackWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productPackWeight));

            RuleFor(x => x.packQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.packQuantity));

            RuleFor(x => x.isUpdate)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isUpdate));

            RuleFor(x => x.index)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.index));

            RuleFor(x => x.bonusQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.bonusQuantity));

        }
    }
}
