using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// campos que a Movidesk exige juntos: status com justificativa (vazia quando o status não pede) e responsável com equipe
    /// </summary>
    public class TicketPairsValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.Ticket>
    {
        public TicketPairsValidator()
        {
            RuleFor(x => x.justification)
                .NotNull()
                .WithMessage("Ao enviar 'status', envie também 'justification' (vazia quando o status não exige justificativa).")
                .When(x => x.status is not null);

            RuleFor(x => x.status)
                .NotNull()
                .WithMessage("Ao enviar 'justification', envie também 'status'.")
                .When(x => x.justification is not null);

            RuleFor(x => x.ownerTeam)
                .NotNull()
                .WithMessage("Ao enviar 'owner', envie também 'ownerTeam'.")
                .When(x => x.owner is not null);

            RuleFor(x => x.owner)
                .NotNull()
                .WithMessage("Ao enviar 'ownerTeam', envie também 'owner'.")
                .When(x => x.ownerTeam is not null);
        }
    }
}
