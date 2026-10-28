using Domain.Totvs.Dtos.Writes.People.Classifications;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Classifications
{
    /// <summary>
    /// valida o corpo da remoção de classificações no controller (corpo inválido volta 400): a pessoa e pelo menos uma classificação, cada uma
    /// com o tipo e o código (o código nulo pode tirar todas do tipo: exigido até a homologação confirmar), sem repetição
    /// </summary>
    public class RemoveClassificationRelationshipValidator : AbstractValidator<RemoveClassificationRelationship>
    {
        public RemoveClassificationRelationshipValidator()
        {
            RuleFor(x => x.personCode)
                .NotNull()
                .WithMessage("O campo 'personCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'personCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.classifications)
                .NotEmpty()
                .WithMessage("Informe pelo menos uma classificação em 'classifications'.")
                .Must(l => l!.Where(c => c is not null).GroupBy(c => (c.classificationTypeCode, c.classificationCode)).All(g => g.Count() == 1))
                .WithMessage("A mesma classificação aparece mais de uma vez em 'classifications'.")
                .When(x => x.classifications is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.classifications)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.classificationTypeCode)
                        .NotNull()
                        .WithMessage("O campo 'classificationTypeCode' é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'classificationTypeCode' deve ter de 1 a 4 dígitos.");

                    c.RuleFor(x => x.classificationCode)
                        .NotEmpty()
                        .WithMessage("O campo 'classificationCode' é obrigatório (sem ele a Totvs pode tirar todas as classificações do tipo).")
                        .MaximumLength(10)
                        .WithMessage("O campo 'classificationCode' aceita até 10 caracteres.");
                })
                .When(x => x.classifications is not null);
        }
    }
}
