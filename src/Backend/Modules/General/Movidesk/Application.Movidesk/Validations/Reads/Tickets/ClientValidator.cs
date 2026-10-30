using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Tickets
{
    public class ClientValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.Client>
    {
        public ClientValidator()
        {
            RuleFor(x => x.id)
                .MaximumLength(64)
                .WithMessage("O campo 'id' do cliente deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.isDeleted)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDeleted));

            RuleFor(x => x.organization)
                .SetValidator(new TicketPersonValidator()!)
                .When(x => x.organization is not null);
        }
    }
}
