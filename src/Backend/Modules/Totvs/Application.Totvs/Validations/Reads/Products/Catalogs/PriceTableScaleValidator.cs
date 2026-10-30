using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class PriceTableScaleValidator : AbstractValidator<PriceTableScale>
    {
        public PriceTableScaleValidator()
        {
            RuleFor(x => x.scaleCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'scaleCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.scaleDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'scaleDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.scaleDescription));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleForEach(x => x.scales)
                .SetValidator(new PriceTableScaleItemValidator());
        }
    }
}
