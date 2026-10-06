using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class ClientValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Client>
    {
        public ClientValidator()
        {
            RuleFor(dto => dto.isDeleted)
                   .MustBeValidBoolean();

            RuleFor(x => x.organization)
                    .SetValidator(new CreatedbyValidator());
        }
    }
}
