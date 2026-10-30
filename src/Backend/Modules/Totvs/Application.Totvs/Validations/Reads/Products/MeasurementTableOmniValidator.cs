using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
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
