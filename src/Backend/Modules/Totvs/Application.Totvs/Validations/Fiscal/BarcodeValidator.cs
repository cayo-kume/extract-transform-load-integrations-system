using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class BarcodeValidator : AbstractValidator<Barcode>
    {
        public BarcodeValidator()
        {
            RuleFor(x => x.barcode)
                .MaximumLength(60)
                .WithMessage("O campo 'barcode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.barcode));
        }
    }
}
