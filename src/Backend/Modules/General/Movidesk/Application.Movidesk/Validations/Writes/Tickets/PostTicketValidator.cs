using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Tickets
{
    /// <summary>
    /// criação de ticket (POST): barra a chamada antes da API, porque 3 erros seguidos bloqueiam a API de tickets por 60 s
    /// </summary>
    public class PostTicketValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Tickets.Ticket>
    {
        public PostTicketValidator()
        {
            RuleFor(x => x.type)
                .Must(x => x is 1 or 2)
                .WithMessage("O campo 'type' é obrigatório: 1 (interno) ou 2 (público).");

            RuleFor(x => x.createdBy)
                .Must(x => !string.IsNullOrWhiteSpace(x?.id))
                .WithMessage("O campo 'createdBy.id' (Cód. Ref. de quem gerou o ticket) é obrigatório.");

            RuleFor(x => x.clients)
                .NotEmpty()
                .WithMessage("O ticket precisa de ao menos um cliente em 'clients'.");

            RuleForEach(x => x.clients)
                .Must(x => !string.IsNullOrWhiteSpace(x?.id))
                .WithMessage("Todo cliente precisa do 'id' (Cód. Ref.).");

            RuleFor(x => x.actions)
                .NotEmpty()
                .WithMessage("O ticket precisa de ao menos uma ação em 'actions'.");

            // na criação todas as ações são novas
            RuleForEach(x => x.actions)
                .Must(x => !(x?.id > 0))
                .WithMessage("Na criação do ticket as ações não podem ter 'id'.")
                .SetValidator(new ActionValidator());

            RuleForEach(x => x.customFieldValues)
                .SetValidator(new CustomFieldValueValidator());

            Include(new TicketPairsValidator());
        }
    }
}
