using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script045_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.name)
                .MaximumLength(100)
                .WithMessage("O campo 'name' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.parentCategoryCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.parentCategoryCode));

            RuleFor(x => x.categoryType)
                .MaximumLength(30)
                .WithMessage("O campo 'categoryType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.categoryType));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
