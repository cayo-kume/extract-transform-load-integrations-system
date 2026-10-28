using Application.Totvs.Services;
using Domain.Totvs.Dtos.Writes.Products.Classifications;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Classifications
{
    /// <summary>
    /// valida o corpo do tipo de classificação no controller (corpo inválido volta 400): o tipo (nunca o 6000, da marca: PR10), tamanhos e
    /// classificações sem repetição. O swagger não dá tamanho às classificações: valem 10, 40 e 100 dos outros modelos
    /// </summary>
    public class UpsertTypeClassificationValidator : AbstractValidator<UpsertTypeClassification>
    {
        public UpsertTypeClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .NotNull()
                .WithMessage("O campo 'typeCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'typeCode' deve ter de 1 a 4 dígitos.")
                .NotEqual(TotvsProductBrands.ClassificationType)
                .WithMessage($"O tipo {TotvsProductBrands.ClassificationType} (marca) só muda pelo ERP.");

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' aceita até 40 caracteres.");

            RuleFor(x => x.auxiliaryDescription)
                .MaximumLength(100)
                .WithMessage("O campo 'auxiliaryDescription' aceita até 100 caracteres.");

            RuleFor(x => x.classifications)
                .Must(l => l!.Where(c => c?.code is not null).GroupBy(c => c.code!.Trim().ToUpperInvariant()).All(g => g.Count() == 1))
                .WithMessage("A mesma classificação ('code') aparece mais de uma vez em 'classifications'.")
                .When(x => x.classifications is not null);

            RuleForEach(x => x.classifications)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.code)
                        .NotEmpty()
                        .WithMessage("O campo 'code' da classificação é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O campo 'code' da classificação aceita até 10 caracteres.");

                    c.RuleFor(x => x.description)
                        .MaximumLength(40)
                        .WithMessage("O campo 'description' da classificação aceita até 40 caracteres.");

                    c.RuleFor(x => x.auxiliaryDescription)
                        .MaximumLength(100)
                        .WithMessage("O campo 'auxiliaryDescription' da classificação aceita até 100 caracteres.");
                })
                .When(x => x.classifications is not null);
        }
    }
}
