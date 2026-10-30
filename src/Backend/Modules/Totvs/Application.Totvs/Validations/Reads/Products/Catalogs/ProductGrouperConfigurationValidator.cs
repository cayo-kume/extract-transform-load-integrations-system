using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class ProductGrouperConfigurationValidator : AbstractValidator<ProductGrouperConfiguration>
    {
        public ProductGrouperConfigurationValidator()
        {
            RuleFor(x => x.grouperCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'grouperCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.type)
                .MaximumLength(30)
                .WithMessage("O campo 'type' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleForEach(x => x.productGrouperItemConfigurationList)
                .SetValidator(new ProductGrouperConfigurationItemValidator());
        }
    }
}
