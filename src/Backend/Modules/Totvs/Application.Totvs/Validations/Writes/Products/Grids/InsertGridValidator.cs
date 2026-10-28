using Domain.Totvs.Dtos.Writes.Products.Grids;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Grids
{
    /// <summary>
    /// valida o corpo da inclusão de grade no controller, antes de chamar a Totvs (corpo inválido volta 400). Obrigatórios e tamanhos do
    /// swagger da API Product (InsertGridCommand); a grade não tem rota de alteração nem de exclusão, então um tamanho repetido ficaria no ERP
    /// </summary>
    public class InsertGridValidator : AbstractValidator<InsertGrid>
    {
        public InsertGridValidator()
        {
            RuleFor(x => x.gridCode)
                .InclusiveBetween(1, 999_999)
                .WithMessage("O campo 'gridCode' deve ter de 1 a 6 dígitos.")
                .When(x => x.gridCode is not null);

            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("O campo 'name' é obrigatório.")
                .MaximumLength(40)
                .WithMessage("O campo 'name' aceita até 40 caracteres.");

            RuleFor(x => x.type)
                .MaximumLength(40)
                .WithMessage("O campo 'type' aceita até 40 caracteres.");

            RuleFor(x => x.gridSizes)
                .NotEmpty()
                .WithMessage("Informe pelo menos um tamanho em 'gridSizes'.");

            RuleFor(x => x.gridSizes)
                .Must(list => list!.Select(s => s.name?.Trim().ToUpperInvariant()).Distinct().Count() == list!.Count)
                .WithMessage("O mesmo tamanho aparece mais de uma vez em 'gridSizes'.")
                .When(x => x.gridSizes is { Count: > 0 });

            RuleForEach(x => x.gridSizes)
                .ChildRules(size =>
                {
                    size.RuleFor(s => s.name)
                        .NotEmpty()
                        .WithMessage("O campo 'name' do tamanho é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O campo 'name' do tamanho aceita até 10 caracteres.");
                })
                .When(x => x.gridSizes is not null);
        }
    }
}
