using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script045_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class PriceTableValidator : AbstractValidator<PriceTable>
    {
        public PriceTableValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(60)
                .WithMessage("O campo 'description' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.codeName)
                .MaximumLength(20)
                .WithMessage("O campo 'codeName' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codeName));

            RuleFor(x => x.type)
                .MaximumLength(40)
                .WithMessage("O campo 'type' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleFor(x => x.startDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.startDate));

            RuleFor(x => x.endDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.endDate));

            RuleFor(x => x.variationValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.variationValue));

            RuleFor(x => x.variationPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.variationPercentage));

            RuleForEach(x => x.salesOrderClassification)
                .SetValidator(new PriceTableSalesOrderClassificationValidator());

            RuleForEach(x => x.personClassification)
                .SetValidator(new PriceTablePersonClassificationValidator());

            RuleForEach(x => x.paymentConditions)
                .SetValidator(new PriceTablePaymentConditionValidator());

            RuleForEach(x => x.averagePeriod)
                .SetValidator(new PriceTableAveragePeriodValidator());

            RuleForEach(x => x.averagePeriodQuantity)
                .SetValidator(new PriceTableAveragePeriodQuantityValidator());
        }
    }
}
