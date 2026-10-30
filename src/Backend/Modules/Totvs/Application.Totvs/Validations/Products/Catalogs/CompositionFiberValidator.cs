using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class CompositionFiberValidator : AbstractValidator<CompositionFiber>
    {
        public CompositionFiberValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.percentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentage));
        }
    }
}
