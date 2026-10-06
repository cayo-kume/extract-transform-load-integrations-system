using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ColorValidator : AbstractValidator<Color>
    {
        public ColorValidator()
        {
			RuleFor(x => x.code)
				.MaximumLength(60)
				.WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.code));

			RuleFor(x => x.name)
				.MaximumLength(60)
				.WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.name));

			RuleFor(x => x.groupName)
				.MaximumLength(60)
				.WithMessage("O campo 'groupName' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.groupName));

			RuleFor(x => x.auxiliaryCode)
				.MaximumLength(60)
				.WithMessage("O campo 'auxiliaryCode' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.auxiliaryCode));

			RuleFor(x => x.auxiliaryName)
				.MaximumLength(60)
				.WithMessage("O campo 'auxiliaryName' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.auxiliaryName));

			RuleFor(x => x.pantoneCode)
				.MaximumLength(60)
				.WithMessage("O campo 'pantoneCode' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.pantoneCode));

			RuleFor(x => x.configurableField)
				.MaximumLength(60)
				.WithMessage("O campo 'configurableField' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.configurableField));

            RuleForEach(x => x.products)
                .SetValidator(new ProductValidator());

            RuleForEach(x => x.measurementTableOmni)
                .SetValidator(new MeasurementTableOmniValidator());
        }
    }
}
