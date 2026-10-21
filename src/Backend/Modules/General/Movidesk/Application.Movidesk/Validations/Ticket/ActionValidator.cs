using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class ActionValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.Action>
    {
        public ActionValidator()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.type)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.origin)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.origin));

            // description e htmlDescription são nvarchar(max): sem limite de tamanho

            RuleFor(x => x.status)
                .MaximumLength(128)
                .WithMessage("O campo 'status' da ação deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.justification)
                .MaximumLength(128)
                .WithMessage("O campo 'justification' da ação deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.justification));

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.isDeleted)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDeleted));

            RuleFor(x => x.createdBy)
                .SetValidator(new TicketPersonValidator()!)
                .When(x => x.createdBy is not null);

            RuleForEach(x => x.attachments)
                .SetValidator(new AttachmentValidator());
        }
    }
}
