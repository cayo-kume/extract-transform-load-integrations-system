using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order e das colunas do Script017_Create_Totvs_Sales_Orders_Tables
    /// </summary>
    public class SaleDiscountTypeValidator : AbstractValidator<SaleDiscountType>
    {
        public SaleDiscountTypeValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(100)
                .WithMessage("O campo 'description' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.percentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentage));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
