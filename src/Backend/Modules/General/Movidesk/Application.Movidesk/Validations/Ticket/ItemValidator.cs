using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class ItemValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Item>
    {
        public ItemValidator()
        {
            RuleFor(x => x.personId)
                .MaximumLength(60)
                .WithMessage("O campo 'personId' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personId));

            RuleFor(x => x.clientId)
                .MaximumLength(60)
                .WithMessage("O campo 'clientId' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.clientId));

            RuleFor(x => x.team)
                .MaximumLength(60)
                .WithMessage("O campo 'team' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.team));

            RuleFor(x => x.customFieldItem)
                .MaximumLength(300)
                .WithMessage("O campo 'customFieldItem' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customFieldItem));
        }
    }
}
