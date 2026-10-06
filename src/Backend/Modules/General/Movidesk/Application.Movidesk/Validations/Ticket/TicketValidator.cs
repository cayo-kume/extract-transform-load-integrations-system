using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Ticket
{
    public class TicketValidator : AbstractValidator<Domain.Movidesk.Dtos.Tickets.Ticket>
    {
        public TicketValidator()
        {
            RuleFor(x => x.id)
                .MaximumLength(10)
                .WithMessage("O campo 'id' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.protocol)
                .MaximumLength(30)
                .WithMessage("O campo 'protocol' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.protocol));

            RuleFor(x => x.type)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.subject)
                .MaximumLength(350)
                .WithMessage("O campo 'subject' deve ter no máximo 350 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.subject));

            RuleFor(x => x.category)
                .MaximumLength(128)
                .WithMessage("O campo 'category' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.category));

            RuleFor(x => x.urgency)
                .MaximumLength(128)
                .WithMessage("O campo 'urgency' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.urgency));

            RuleFor(x => x.status)
                .MaximumLength(128)
                .WithMessage("O campo 'status' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.baseStatus)
                .MaximumLength(128)
                .WithMessage("O campo 'baseStatus' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.baseStatus));

            RuleFor(x => x.justification)
                .MaximumLength(128)
                .WithMessage("O campo 'justification' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.justification));

            RuleFor(x => x.origin)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.origin));

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.originEmailAccount)
                .MaximumLength(128)
                .WithMessage("O campo 'originEmailAccount' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.originEmailAccount));

            //RuleFor(x => x.owner)
            //    .MaximumLength(64)
            //    .WithMessage("O campo 'owner' deve ter no máximo 64 caracteres.")
            //    .When(x => !string.IsNullOrEmpty(x.owner));

            RuleFor(x => x.lifeTimeWorkingTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.lifeTimeWorkingTime));

            RuleFor(x => x.stoppedTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.stoppedTime));

            RuleFor(x => x.stoppedTimeWorkingTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.stoppedTimeWorkingTime));

            RuleFor(x => x.chatTalkTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.chatTalkTime));

            RuleFor(x => x.chatWaitingTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.chatWaitingTime));

            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.slaSolutionTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.slaSolutionTime));

            //RuleFor(x => x.slaResponseTime)
            //    .MustBeValidInt32()
            //    .When(x => !string.IsNullOrEmpty(x.slaResponseTime));

            RuleFor(x => x.serviceFirstLevelId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.serviceFirstLevelId));

            RuleFor(x => x.actionCount)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.actionCount));

            RuleFor(x => x.protocol)
                .MaximumLength(60)
                .WithMessage("O campo 'protocol' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.protocol));

            RuleFor(x => x.subject)
                .MaximumLength(60)
                .WithMessage("O campo 'subject' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.subject));

            RuleFor(x => x.category)
                .MaximumLength(60)
                .WithMessage("O campo 'category' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.category));

            RuleFor(x => x.urgency)
                .MaximumLength(60)
                .WithMessage("O campo 'urgency' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.urgency));

            RuleFor(x => x.status)
                .MaximumLength(60)
                .WithMessage("O campo 'status' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.baseStatus)
                .MaximumLength(60)
                .WithMessage("O campo 'baseStatus' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.baseStatus));

            RuleFor(x => x.justification)
                .MaximumLength(60)
                .WithMessage("O campo 'justification' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.justification));

            RuleFor(x => x.serviceFirstLevel)
                .MaximumLength(60)
                .WithMessage("O campo 'serviceFirstLevel' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serviceFirstLevel));

            RuleFor(x => x.serviceSecondLevel)
                .MaximumLength(60)
                .WithMessage("O campo 'serviceSecondLevel' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serviceSecondLevel));

            RuleFor(x => x.serviceThirdLevel)
                .MaximumLength(60)
                .WithMessage("O campo 'serviceThirdLevel' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serviceThirdLevel));

            RuleFor(x => x.contactForm)
                .MaximumLength(60)
                .WithMessage("O campo 'contactForm' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contactForm));

            RuleFor(x => x.cc)
                .MaximumLength(300)
                .WithMessage("O campo 'cc' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cc));

            RuleFor(x => x.originEmailAccount)
                .MaximumLength(120)
                .WithMessage("O campo 'originEmailAccount' deve ter no máximo 120 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.originEmailAccount));

            RuleFor(x => x.chatWidget)
                .MaximumLength(60)
                .WithMessage("O campo 'chatWidget' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chatWidget));

            RuleFor(x => x.chatGroup)
                .MaximumLength(60)
                .WithMessage("O campo 'chatGroup' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chatGroup));

            RuleFor(x => x.slaAgreement)
                .MaximumLength(50)
                .WithMessage("O campo 'slaAgreement' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.slaAgreement));

            RuleFor(x => x.slaAgreementRule)
                .MaximumLength(50)
                .WithMessage("O campo 'slaAgreementRule' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.slaAgreementRule));

            RuleFor(x => x.ownerTeam)
                .MaximumLength(50)
                .WithMessage("O campo 'ownerTeam' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ownerTeam));

            RuleFor(x => String.Join(",", x.serviceFull))
                .MaximumLength(300)
                .WithMessage("O campo 'serviceFull' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(String.Join(",", x.serviceFull)));

            RuleFor(x => String.Join(",", x.tags))
                .MaximumLength(300)
                .WithMessage("O campo 'tags' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(String.Join(",", x.tags)));

            RuleFor(x => x.slaSolutionDate)
                .MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.slaSolutionDate));

            RuleFor(x => x.slaResponseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.slaResponseDate));

            RuleFor(x => x.slaRealResponseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.slaRealResponseDate));

            RuleFor(x => x.createdDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.createdDate));

            RuleFor(x => x.resolvedIn)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.resolvedIn));

            RuleFor(x => x.reopenedIn)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.reopenedIn));

            RuleFor(x => x.closedIn)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.closedIn));

            RuleFor(x => x.lastActionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastActionDate));

            RuleFor(x => x.lastUpdate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastUpdate));

            RuleFor(x => x.slaSolutionChangedByUser)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.slaSolutionChangedByUser));

            RuleFor(x => x.resolvedInFirstCall)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.resolvedInFirstCall));

            RuleFor(x => x.slaSolutionDateIsPaused)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.slaSolutionDateIsPaused));

            RuleFor(x => x.owner)
                .SetValidator(new CreatedbyValidator());

            RuleFor(x => x.createdBy)
                .SetValidator(new CreatedbyValidator());

            RuleFor(x => x.slaSolutionChangedBy)
                .SetValidator(new CreatedbyValidator());

            RuleForEach(x => x.clients)
                .SetValidator(new ClientValidator());

            RuleForEach(x => x.actions)
                .SetValidator(new ActionValidator());
        }
    }
}
