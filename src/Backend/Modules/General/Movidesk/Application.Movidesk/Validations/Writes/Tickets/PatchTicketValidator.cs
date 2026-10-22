using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// alteração de ticket (PATCH): ao menos um campo, e os campos que andam em par vão juntos.
    /// Listas enviadas substituem a lista inteira na Movidesk (quem chama manda a lista completa).
    /// </summary>
    public class PatchTicketValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.Ticket>
    {
        public PatchTicketValidator()
        {
            RuleFor(x => x)
                .Must(HasAnyField)
                .WithMessage("Informe ao menos um campo para alterar.");

            RuleFor(x => x.type)
                .Must(x => x is 1 or 2)
                .WithMessage("O campo 'type' deve ser 1 (interno) ou 2 (público).")
                .When(x => x.type is not null);

            RuleForEach(x => x.clients)
                .Must(x => !string.IsNullOrWhiteSpace(x?.id))
                .WithMessage("Todo cliente precisa do 'id' (Cód. Ref.).");

            RuleForEach(x => x.actions)
                .SetValidator(new ActionValidator());

            RuleForEach(x => x.customFieldValues)
                .SetValidator(new CustomFieldValueValidator());

            Include(new TicketPairsValidator());
        }

        private static bool HasAnyField(Domain.Movidesk.Dtos.Writes.Tickets.Ticket x) =>
            typeof(Domain.Movidesk.Dtos.Writes.Tickets.Ticket).GetProperties().Any(p => p.GetValue(x) is not null);
    }
}
