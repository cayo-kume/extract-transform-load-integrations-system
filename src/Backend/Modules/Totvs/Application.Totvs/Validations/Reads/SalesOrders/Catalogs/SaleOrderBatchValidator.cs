using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order e das colunas do Script017_Create_Totvs_Sales_Orders_Tables
    /// </summary>
    public class SaleOrderBatchValidator : AbstractValidator<SaleOrderBatch>
    {
        public SaleOrderBatchValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório: o registro não é gravado.")
                .MaximumLength(10)
                .WithMessage("O campo 'code' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.description)
                .MaximumLength(60)
                .WithMessage("O campo 'description' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.startDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.startDate));

            RuleFor(x => x.endDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.endDate));

            RuleFor(x => x.closingDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.closingDate));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
