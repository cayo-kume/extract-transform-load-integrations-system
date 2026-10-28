using Domain.Totvs.Dtos.Writes.Products.Batches;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Batches
{
    /// <summary>
    /// valida o corpo da alteração de itens de lote no controller (corpo inválido volta 400): cada item por loja + lote + sequência ou pelo lote
    /// do cliente, um ou outro (regra da descrição), com pelo menos um dado a mudar
    /// </summary>
    public class UpdateBatchInfoValidator : AbstractValidator<UpdateBatchInfo>
    {
        public UpdateBatchInfoValidator()
        {
            RuleFor(x => x.batchItems)
                .NotEmpty()
                .WithMessage("Informe pelo menos um item em 'batchItems'.");

            RuleForEach(x => x.batchItems)
                .ChildRules(i =>
                {
                    i.RuleFor(x => x)
                        .Must(x => (x.branchCode is not null && x.batchCode is not null && x.sequence is not null && String.IsNullOrEmpty(x.itemClient))
                            || (x.branchCode is null && x.batchCode is null && x.sequence is null && !String.IsNullOrEmpty(x.itemClient)))
                        .WithName("branchCode")
                        .WithMessage("Informe o item por 'branchCode' + 'batchCode' + 'sequence' ou por 'itemClient', um ou outro.");

                    i.RuleFor(x => x.branchCode)
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.")
                        .When(x => x.branchCode is not null);

                    i.RuleFor(x => x.itemClient).MaximumLength(40).WithMessage("O campo 'itemClient' aceita até 40 caracteres.");

                    i.RuleFor(x => x.dataChanged)
                        .NotNull()
                        .WithMessage("O campo 'dataChanged' é obrigatório.");

                    i.RuleFor(x => x.dataChanged!)
                        .ChildRules(d =>
                        {
                            d.RuleFor(x => x)
                                .Must(x => new object?[] { x.nuance, x.location, x.itemClient, x.itemProvider, x.branchSalesOrder, x.salesOrder, x.additionalFields, x.conditioningType,
                                    x.quantityItem, x.quantityYield, x.quantityMeters, x.supplierWidth, x.realWidth, x.grammage }.Any(v => v is not null))
                                .WithName("dataChanged")
                                .WithMessage("Informe pelo menos um dado a mudar em 'dataChanged'.");

                            d.RuleFor(x => x)
                                .Must(x => (x.branchSalesOrder is null) == (x.salesOrder is null))
                                .WithName("dataChanged.branchSalesOrder")
                                .WithMessage("Os campos 'branchSalesOrder' e 'salesOrder' vão juntos.");

                            d.RuleFor(x => x.nuance).MaximumLength(20).WithMessage("O campo 'nuance' aceita até 20 caracteres.");
                            d.RuleFor(x => x.location).MaximumLength(5).WithMessage("O campo 'location' aceita até 5 caracteres.");
                            d.RuleFor(x => x.itemClient).MaximumLength(40).WithMessage("O campo 'dataChanged.itemClient' aceita até 40 caracteres.");
                            d.RuleFor(x => x.itemProvider).MaximumLength(40).WithMessage("O campo 'itemProvider' aceita até 40 caracteres.");
                            d.RuleFor(x => x.conditioningType).MaximumLength(10).WithMessage("O campo 'conditioningType' aceita até 10 caracteres.");

                            d.RuleFor(x => x)
                                .Must(x => new decimal?[] { x.quantityItem, x.quantityYield, x.quantityMeters, x.supplierWidth, x.realWidth }.All(q => q is null or >= 0))
                                .WithName("dataChanged.quantityItem")
                                .WithMessage("As quantidades e larguras não podem ser negativas.");

                            d.RuleForEach(x => x.additionalFields)
                                .SetValidator(new BatchAdditionalFieldValidator())
                                .When(x => x.additionalFields is not null);
                        })
                        .When(x => x.dataChanged is not null);
                })
                .When(x => x.batchItems is not null);
        }
    }
}
