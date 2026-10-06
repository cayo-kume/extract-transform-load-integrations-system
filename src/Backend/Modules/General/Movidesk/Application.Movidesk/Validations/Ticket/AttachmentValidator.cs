using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class AttachmentValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Attachment>
    {
        public AttachmentValidator()
        {
            RuleFor(dto => dto.fileName)
                   .MaximumLength(120);

            RuleFor(dto => dto.path)
                   .MaximumLength(300);

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.createdBy)
                .SetValidator(new CreatedbyValidator());
        }
    }
}
