using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class ActionValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Action>
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

            RuleFor(dto => dto.description)
                   .MaximumLength(300);

            RuleFor(dto => dto.status)
                   .MaximumLength(60);

            RuleFor(dto => dto.justification)
                   .MaximumLength(300);

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(dto => dto.isDeleted)
                   .MustBeValidBoolean();

            RuleFor(x => x.createdBy)
                .SetValidator(new CreatedbyValidator());

            RuleForEach(x => x.attachments)
                .SetValidator(new AttachmentValidator());

            RuleForEach(x => x.customFieldValues)
                .SetValidator(new CustomfieldValueValidator());
        }
    }
}
