using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Batches;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Batches
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product (SimpleBatchProductModel)
    /// </summary>
    public class BatchValidator : AbstractValidator<Batch>
    {
        public BatchValidator()
        {
            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            // anulável no swagger, mas é a chave: lote sem código não é gravado
            RuleFor(x => x.batchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'batchCode' veio vazio: o lote não é gravado.")
                .MustBeValidInt32();

            RuleFor(x => x.batchProvider)
                .MaximumLength(40)
                .WithMessage("O campo 'batchProvider' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.batchProvider));

            RuleFor(x => x.batchClient)
                .MaximumLength(40)
                .WithMessage("O campo 'batchClient' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.batchClient));

            RuleForEach(x => x.batchItem)
                .Must(x => !string.IsNullOrEmpty(x.batchItemSequential))
                .WithMessage("Item de lote sem 'batchItemSequential': descartado.");
        }
    }
}
