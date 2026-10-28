using Application.Totvs.Validations.Writes.People.Customers;
using Domain.Totvs.Dtos.Writes.Products.Batches;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Batches
{
    /// <summary>
    /// valida o corpo da inclusão de lotes no controller (corpo inválido volta 400): loja, produto e saldo de cada lote, itens com tamanhos do
    /// swagger, pedido de venda com empresa e número juntos e a nota de inspeção com o fornecedor por código ou por CPF/CNPJ
    /// </summary>
    public class InsertBatchesValidator : AbstractValidator<InsertBatches>
    {
        public InsertBatchesValidator()
        {
            RuleFor(x => x.batchs)
                .NotEmpty()
                .WithMessage("Informe pelo menos um lote em 'batchs'.");

            RuleForEach(x => x.batchs)
                .ChildRules(b =>
                {
                    b.RuleFor(x => x.branchCode)
                        .NotNull()
                        .WithMessage("O campo 'branchCode' do lote é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'branchCode' do lote deve ter de 1 a 4 dígitos.");

                    b.RuleFor(x => x.productCode)
                        .NotNull()
                        .WithMessage("O campo 'productCode' do lote é obrigatório.")
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'productCode' do lote deve ter de 1 a 9 dígitos.");

                    b.RuleFor(x => x.balanceCode)
                        .NotNull()
                        .WithMessage("O campo 'balanceCode' é obrigatório.")
                        .InclusiveBetween(1, 999)
                        .WithMessage("O campo 'balanceCode' deve ter de 1 a 3 dígitos.");

                    b.RuleFor(x => x.batchCode)
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'batchCode' deve ter de 1 a 9 dígitos (ou ficar fora: a Totvs gera).")
                        .When(x => x.batchCode is not null);

                    b.RuleFor(x => x.batchProvider).MaximumLength(40).WithMessage("O campo 'batchProvider' aceita até 40 caracteres.");
                    b.RuleFor(x => x.batchClient).MaximumLength(40).WithMessage("O campo 'batchClient' aceita até 40 caracteres.");
                    b.RuleFor(x => x.batchType).IsInEnum().WithMessage("O campo 'batchType' só aceita de 1 a 5.");

                    b.RuleForEach(x => x.batchItem)
                        .SetValidator(new BatchItemValidator())
                        .When(x => x.batchItem is not null);
                })
                .When(x => x.batchs is not null);
        }
    }
}
