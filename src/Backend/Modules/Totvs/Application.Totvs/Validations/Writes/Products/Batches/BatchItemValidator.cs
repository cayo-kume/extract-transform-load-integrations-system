using Application.Totvs.Validations.Writes.People.Customers;
using Domain.Totvs.Dtos.Writes.Products.Batches;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Batches
{
    /// <summary>
    /// item do lote: situação, quantidades não negativas, textos do swagger, pedido de venda e nota de inspeção
    /// </summary>
    public class BatchItemValidator : AbstractValidator<BatchItem>
    {
        public BatchItemValidator()
        {
            RuleFor(x => x.status).IsInEnum().WithMessage("O campo 'status' do item de lote só aceita 1, 2, 4 ou 10.");
            RuleFor(x => x.itemProvider).MaximumLength(40).WithMessage("O campo 'itemProvider' aceita até 40 caracteres.");
            RuleFor(x => x.itemClient).MaximumLength(40).WithMessage("O campo 'itemClient' aceita até 40 caracteres.");
            RuleFor(x => x.nuance).MaximumLength(20).WithMessage("O campo 'nuance' aceita até 20 caracteres.");
            RuleFor(x => x.locationBatch).MaximumLength(5).WithMessage("O campo 'locationBatch' aceita até 5 caracteres.");
            RuleFor(x => x.conditioningType).MaximumLength(10).WithMessage("O campo 'conditioningType' aceita até 10 caracteres.");

            RuleFor(x => x.batchItemSequential)
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'batchItemSequential' deve ter de 1 a 4 dígitos.")
                .When(x => x.batchItemSequential is not null);

            RuleFor(x => x)
                .Must(x => new decimal?[] { x.quantityItem, x.quantityTare, x.quantityWeigth, x.quantityMeters, x.quantityCone, x.quantityYield }.All(q => q is null or >= 0))
                .WithName("quantityItem")
                .WithMessage("As quantidades do item de lote não podem ser negativas.");

            RuleFor(x => x)
                .Must(x => (x.branchSalesOrder is null) == (x.salesOrder is null))
                .WithName("branchSalesOrder")
                .WithMessage("Os campos 'branchSalesOrder' e 'salesOrder' vão juntos.");

            RuleFor(x => x.typeTare)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'typeTare' deve ter de 1 a 9 dígitos.")
                .When(x => x.typeTare is not null);

            RuleForEach(x => x.additionalFields)
                .SetValidator(new BatchAdditionalFieldValidator())
                .When(x => x.additionalFields is not null);

            RuleFor(x => x.invoice!)
                .ChildRules(i =>
                {
                    i.RuleFor(x => x)
                        .Must(x => (x.supplierCode is null) != String.IsNullOrEmpty(x.supplierCpfCnpj))
                        .WithName("supplierCode")
                        .WithMessage("Informe exatamente um entre 'invoice.supplierCode' e 'invoice.supplierCpfCnpj'.");

                    i.RuleFor(x => x.supplierCpfCnpj)
                        .Matches(@"^(\d{11}|\d{14})$")
                        .WithMessage("O campo 'invoice.supplierCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                        .When(x => !String.IsNullOrEmpty(x.supplierCpfCnpj));

                    i.RuleFor(x => x.invoiceCode).NotNull().WithMessage("O campo 'invoice.invoiceCode' é obrigatório.");
                    i.RuleFor(x => x.inspectionLayout).NotNull().WithMessage("O campo 'invoice.inspectionLayout' é obrigatório.");
                    i.RuleFor(x => x.quantityInspection).NotNull().WithMessage("O campo 'invoice.quantityInspection' é obrigatório.");

                    i.RuleFor(x => x.inspectionFields)
                        .NotEmpty()
                        .WithMessage("Informe pelo menos um campo em 'invoice.inspectionFields'.");

                    i.RuleForEach(x => x.inspectionFields)
                        .ChildRules(f =>
                        {
                            f.RuleFor(x => x.field)
                                .NotNull()
                                .WithMessage("O campo 'field' da inspeção é obrigatório.")
                                .InclusiveBetween(1, 9_999)
                                .WithMessage("O campo 'field' da inspeção deve ter de 1 a 4 dígitos.");

                            f.RuleFor(x => x.inspectionValue)
                                .NotNull()
                                .WithMessage("O campo 'inspectionValue' é obrigatório.")
                                .Must(CustomerRules.IsAdditionalValue)
                                .WithMessage("O campo 'inspectionValue' aceita texto até 80 caracteres, número com até 12 inteiros e 6 decimais ou data DD/MM/YYYY.");
                        })
                        .When(x => x.inspectionFields is not null);
                })
                .When(x => x.invoice is not null);
        }
    }
}
