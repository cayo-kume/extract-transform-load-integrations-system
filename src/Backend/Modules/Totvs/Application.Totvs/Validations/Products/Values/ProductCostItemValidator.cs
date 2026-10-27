using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Values
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script048_Create_Totvs_Product_Values_Tables (o swagger não dá tamanho aos nomes)
    /// </summary>
    public class ProductCostItemValidator : AbstractValidator<ProductCostItem>
    {
        public ProductCostItemValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.costCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'costCode' é obrigatório.")
                .MustBeValidDecimal();

            RuleFor(x => x.costName)
                .MaximumLength(60)
                .WithMessage("O campo 'costName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.costName));

            RuleFor(x => x.cost)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.cost));
        }
    }
}
