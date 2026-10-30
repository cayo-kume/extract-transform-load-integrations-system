using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Tickets
{
    /// <summary>
    /// pessoa resumida que vem dentro do ticket (owner, createdBy, clients, organization...)
    /// </summary>
    public class TicketPersonValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.Person>
    {
        public TicketPersonValidator()
        {
            RuleFor(x => x.id)
                .MaximumLength(64)
                .WithMessage("O campo 'id' da pessoa deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));
        }
    }
}
