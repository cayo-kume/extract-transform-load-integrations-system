using Application.Totvs.Validations.Person.Customer;
using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class SkuValidator : AbstractValidator<Sku>
    {
        public SkuValidator()
        {
            RuleFor(x => x.ReferenceCode)
                .MaximumLength(60)
                .WithMessage("O campo 'ReferenceCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ReferenceCode));

            RuleFor(x => x.lastReferenceCode)
                .MaximumLength(60)
                .WithMessage("O campo 'lastReferenceCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lastReferenceCode));

            RuleFor(x => x.description)
                .MaximumLength(250)
                .WithMessage("O campo 'description' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.descriptive)
                .MaximumLength(250)
                .WithMessage("O campo 'descriptive' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descriptive));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.referenceId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.referenceId));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.gridCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.gridCode));

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

            RuleForEach(x => x.grid)
                .MaximumLength(60)
                .WithMessage("Cada item do campo 'grid' deve ter no máximo 60 caracteres.")
                .When(x => x.grid != null);

            RuleForEach(x => x.observations)
                .SetValidator(new ObservationValidator());

            RuleForEach(x => x.details)
                .SetValidator(new DetailValidator());

            RuleForEach(x => x.composition)
                .SetValidator(new CompositionValidator());

            RuleForEach(x => x.referenceCategories)
                .SetValidator(new ReferenceCategoryValidator());

            RuleForEach(x => x.colors)
                .SetValidator(new ColorValidator());
        }
    }
}
