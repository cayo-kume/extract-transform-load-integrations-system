using Domain.Totvs.Dtos.Writes.Products.Data;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Data
{
    /// <summary>
    /// valida o corpo dos indicadores dos produtos numa empresa no controller (corpo inválido volta 400): o filtro por lista e pelo menos um
    /// dado
    /// </summary>
    public class UpsertProductBranchInfoValidator : AbstractValidator<UpsertProductBranchInfo>
    {
        public UpsertProductBranchInfoValidator()
        {
            RuleFor(x => x.filter)
                .NotNull()
                .WithMessage("O campo 'filter' é obrigatório (productCodeList ou barCodeList).");

            RuleFor(x => x.filter!)
                .SetValidator(new ProductDataFilterValidator())
                .When(x => x.filter is not null);

            RuleFor(x => x)
                .Must(x => new object?[] { x.isOwnProduction, x.isInactive, x.isFinishedProduct, x.isRawMaterial, x.isBulkMaterial, x.spedItemType }.Any(v => v is not null))
                .WithName("isInactive")
                .WithMessage("Informe pelo menos um dado a mudar.");

            RuleFor(x => x.spedItemType).IsInEnum().WithMessage("O campo 'spedItemType' só aceita de 0 a 10 ou 99.");
        }
    }
}
