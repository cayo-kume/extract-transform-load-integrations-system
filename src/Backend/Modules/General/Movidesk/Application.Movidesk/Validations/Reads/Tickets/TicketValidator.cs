using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Tickets
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os das colunas do Script002_Create_Movidesk_Tickets_Tables
    /// </summary>
    public class TicketValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>
    {
        public TicketValidator()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.protocol)
                .MaximumLength(30)
                .WithMessage("O campo 'protocol' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.protocol));

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

            RuleFor(x => x.originEmailAccount)
                .MaximumLength(128)
                .WithMessage("O campo 'originEmailAccount' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.originEmailAccount));

            RuleFor(x => x.ownerTeam)
                .MaximumLength(128)
                .WithMessage("O campo 'ownerTeam' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ownerTeam));

            RuleFor(x => x.serviceFirstLevel)
                .MaximumLength(1024)
                .WithMessage("O campo 'serviceFirstLevel' deve ter no máximo 1024 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serviceFirstLevel));

            RuleFor(x => x.serviceSecondLevel)
                .MaximumLength(1024)
                .WithMessage("O campo 'serviceSecondLevel' deve ter no máximo 1024 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serviceSecondLevel));

            RuleFor(x => x.serviceThirdLevel)
                .MaximumLength(1024)
                .WithMessage("O campo 'serviceThirdLevel' deve ter no máximo 1024 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serviceThirdLevel));

            RuleFor(x => x.contactForm)
                .MaximumLength(128)
                .WithMessage("O campo 'contactForm' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.contactForm));

            RuleFor(x => x.cc)
                .MaximumLength(1024)
                .WithMessage("O campo 'cc' deve ter no máximo 1024 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cc));

            RuleFor(x => x.chatWidget)
                .MaximumLength(128)
                .WithMessage("O campo 'chatWidget' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chatWidget));

            RuleFor(x => x.chatGroup)
                .MaximumLength(128)
                .WithMessage("O campo 'chatGroup' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chatGroup));

            RuleFor(x => x.slaAgreement)
                .MaximumLength(128)
                .WithMessage("O campo 'slaAgreement' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.slaAgreement));

            RuleFor(x => x.slaAgreementRule)
                .MaximumLength(128)
                .WithMessage("O campo 'slaAgreementRule' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.slaAgreementRule));

            RuleFor(x => x.jiraIssueKey)
                .MaximumLength(64)
                .WithMessage("O campo 'jiraIssueKey' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.jiraIssueKey));

            RuleFor(x => x.type)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.origin)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.origin));

            RuleFor(x => x.serviceFirstLevelId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.serviceFirstLevelId));

            RuleFor(x => x.actionCount)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.actionCount));

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

            RuleFor(x => x.slaResponseTime)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.slaResponseTime));

            RuleFor(x => x.redmineIssueId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.redmineIssueId));

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

            RuleFor(x => x.canceledIn)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.canceledIn));

            RuleFor(x => x.lastActionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastActionDate));

            RuleFor(x => x.lastUpdate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastUpdate));

            RuleFor(x => x.slaSolutionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.slaSolutionDate));

            RuleFor(x => x.slaResponseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.slaResponseDate));

            RuleFor(x => x.slaRealResponseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.slaRealResponseDate));

            RuleFor(x => x.isDeleted)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDeleted));

            RuleFor(x => x.resolvedInFirstCall)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.resolvedInFirstCall));

            RuleFor(x => x.slaSolutionChangedByUser)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.slaSolutionChangedByUser));

            RuleFor(x => x.slaSolutionDateIsPaused)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.slaSolutionDateIsPaused));

            RuleFor(x => x.owner)
                .SetValidator(new TicketPersonValidator()!)
                .When(x => x.owner is not null);

            RuleFor(x => x.createdBy)
                .SetValidator(new TicketPersonValidator()!)
                .When(x => x.createdBy is not null);

            RuleFor(x => x.slaSolutionChangedBy)
                .SetValidator(new TicketPersonValidator()!)
                .When(x => x.slaSolutionChangedBy is not null);

            RuleForEach(x => x.clients)
                .SetValidator(new ClientValidator());

            RuleForEach(x => x.actions)
                .SetValidator(new ActionValidator());

            RuleForEach(x => x.customFieldValues)
                .SetValidator(new CustomFieldValueValidator());
        }
    }
}
