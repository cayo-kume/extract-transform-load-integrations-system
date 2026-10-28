using Domain.Totvs.Dtos.Writes.Products.Data;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Data
{
    /// <summary>
    /// valida o corpo dos dados do produto numa empresa no controller (corpo inválido volta 400): pelo menos um campo, NCM com 8 dígitos, CST
    /// do swagger e peso com 9 inteiros e 3 decimais
    /// </summary>
    public class ProductBranchUpdateValidator : AbstractValidator<ProductBranchUpdate>
    {
        public ProductBranchUpdateValidator()
        {
            RuleFor(x => x)
                .Must(x => new object?[] { x.ncmCode, x.cstCode, x.weight, x.isOwnProduction, x.isInactive, x.isFinishedProduct, x.isRawMaterial, x.isBulkMaterial }.Any(v => v is not null))
                .WithName("ncmCode")
                .WithMessage("Informe pelo menos um campo a mudar.");

            RuleFor(x => x.ncmCode)
                .Matches(@"^\d{8}$")
                .WithMessage("O campo 'ncmCode' deve ter 8 dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.ncmCode));

            RuleFor(x => x.cstCode).IsInEnum().WithMessage("O campo 'cstCode' só aceita de 0 a 8.");

            RuleFor(x => x.weight)
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'weight' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'weight' aceita até 3 casas decimais.")
                .When(x => x.weight is not null);
        }
    }
}
