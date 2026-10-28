using Domain.Totvs.Dtos.Writes.Products.Data;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Data
{
    /// <summary>
    /// valida o corpo dos dados fiscais dos produtos no controller (corpo inválido volta 400): o filtro por lista, pelo menos um dado e o prefixo
    /// EAN nunca vazio (a Totvs não aceita remoção)
    /// </summary>
    public class UpdateProductDataValidator : AbstractValidator<UpdateProductData>
    {
        public UpdateProductDataValidator()
        {
            RuleFor(x => x.filter)
                .NotNull()
                .WithMessage("O campo 'filter' é obrigatório (productCodeList ou barCodeList).");

            RuleFor(x => x.filter!)
                .SetValidator(new ProductDataFilterValidator())
                .When(x => x.filter is not null);

            RuleFor(x => x)
                .Must(x => new object?[] { x.ncmCode, x.measuredUnit, x.cstCode, x.weight, x.cestCode, x.prefixEanGtin }.Any(v => v is not null))
                .WithName("ncmCode")
                .WithMessage("Informe pelo menos um dado a mudar.");

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

            RuleFor(x => x.measuredUnit).MaximumLength(3).WithMessage("O campo 'measuredUnit' aceita até 3 caracteres.");
            RuleFor(x => x.cestCode).MaximumLength(20).WithMessage("O campo 'cestCode' aceita até 20 caracteres.");

            RuleFor(x => x.prefixEanGtin)
                .NotEmpty()
                .WithMessage("O campo 'prefixEanGtin' não aceita remoção: mande o prefixo ou deixe o campo fora.")
                .MaximumLength(20)
                .WithMessage("O campo 'prefixEanGtin' aceita até 20 caracteres.")
                .When(x => x.prefixEanGtin is not null);
        }
    }
}
