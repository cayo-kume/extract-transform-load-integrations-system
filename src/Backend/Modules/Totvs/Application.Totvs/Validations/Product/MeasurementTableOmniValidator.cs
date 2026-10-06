using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class MeasurementTableOmniValidator : AbstractValidator<MeasurementTableOmni>
    {
        public MeasurementTableOmniValidator()
        {
            RuleFor(x => x.sku)
                .MaximumLength(60)
                .WithMessage("O campo 'sku' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sku));

            RuleFor(x => x.tamanho)
                .MaximumLength(60)
                .WithMessage("O campo 'tamanho' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tamanho));

            RuleForEach(x => x.medidas)
                .SetValidator(new MedidaValidator());
        }
    }
}
