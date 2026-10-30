using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class ColorValidator : AbstractValidator<Color>
    {
        public ColorValidator()
        {
            RuleFor(x => x.colorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'colorCode' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'colorCode' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.colorName)
                .MaximumLength(40)
                .WithMessage("O campo 'colorName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorName));

            RuleForEach(x => x.additionalColorInformation)
                .SetValidator(new ColorAdditionalInformationValidator());
        }
    }
}
