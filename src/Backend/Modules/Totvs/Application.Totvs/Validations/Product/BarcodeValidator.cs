using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class BarcodeValidator : AbstractValidator<Barcode>
    {
        public BarcodeValidator()
        {
			RuleFor(x => x.code)
				.MustBeValidInt32()
				.When(x => !string.IsNullOrEmpty(x.code));

			RuleFor(x => x.code)
				.MaximumLength(60)
				.WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.code));

			RuleFor(x => x.quantity)
				.MustBeValidDecimal()
				.When(x => !string.IsNullOrEmpty(x.quantity));

			RuleFor(x => x.isMainCode)
				.MustBeValidBoolean()
				.When(x => !string.IsNullOrEmpty(x.isMainCode));
		}
    }
}
