using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Tickets
{
    public class AttachmentValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.Attachment>
    {
        public AttachmentValidator()
        {
            RuleFor(x => x.fileName)
                .MaximumLength(255)
                .WithMessage("O campo 'fileName' deve ter no máximo 255 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fileName));

            RuleFor(x => x.path)
                .MaximumLength(255)
                .WithMessage("O campo 'path' deve ter no máximo 255 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.path));

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.createdBy)
                .SetValidator(new TicketPersonValidator()!)
                .When(x => x.createdBy is not null);
        }
    }
}
