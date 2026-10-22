using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// campo adicional: o id do campo, o id da regra de exibição e a linha são obrigatórios
    /// </summary>
    public class CustomFieldValueValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.CustomFieldValue>
    {
        public CustomFieldValueValidator()
        {
            RuleFor(x => x.customFieldId)
                .GreaterThan(0)
                .WithMessage("O campo 'customFieldId' é obrigatório.");

            RuleFor(x => x.customFieldRuleId)
                .GreaterThan(0)
                .WithMessage("O campo 'customFieldRuleId' é obrigatório.");

            RuleFor(x => x.line)
                .GreaterThan(0)
                .WithMessage("O campo 'line' é obrigatório (1 quando a regra não permite adicionar linhas).");
        }
    }
}
