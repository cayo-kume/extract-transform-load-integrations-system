using Domain.Totvs.Dtos.Writes.Products.Colors;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Colors
{
    /// <summary>
    /// valida o corpo da inclusão/alteração de cor no controller, antes de chamar a Totvs (corpo inválido volta 400). Obrigatórios e tamanhos
    /// do swagger da API Product (UpsertColorCommand); as lojas são conferidas no service (precisam ser da empresa)
    /// </summary>
    public class UpsertColorValidator : AbstractValidator<UpsertColor>
    {
        public UpsertColorValidator()
        {
            RuleFor(x => x.code)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'code' aceita até 10 caracteres.");

            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("O campo 'name' é obrigatório.")
                .MaximumLength(40)
                .WithMessage("O campo 'name' aceita até 40 caracteres.");

            RuleFor(x => x.colorInfoAdditionals)
                .Must(list => list!.Select(i => i.branchCode).Distinct().Count() == list!.Count)
                .WithMessage("A mesma loja aparece mais de uma vez em 'colorInfoAdditionals'.")
                .When(x => x.colorInfoAdditionals is not null);

            RuleForEach(x => x.colorInfoAdditionals)
                .ChildRules(info =>
                {
                    info.RuleFor(i => i.branchCode)
                        .NotNull()
                        .WithMessage("O campo 'branchCode' é obrigatório.")
                        .InclusiveBetween(1, 9999)
                        .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

                    info.RuleFor(i => i.auxiliaryCode)
                        .MaximumLength(10)
                        .WithMessage("O campo 'auxiliaryCode' aceita até 10 caracteres.");

                    info.RuleFor(i => i.auxiliaryName)
                        .MaximumLength(80)
                        .WithMessage("O campo 'auxiliaryName' aceita até 80 caracteres.");

                    info.RuleFor(i => i.pantoneCode)
                        .MaximumLength(40)
                        .WithMessage("O campo 'pantoneCode' aceita até 40 caracteres.");
                })
                .When(x => x.colorInfoAdditionals is not null);
        }
    }
}
