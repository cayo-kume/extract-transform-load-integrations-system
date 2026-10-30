using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.Services
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os das colunas do Script009_Create_Movidesk_Services_Tables
    /// </summary>
    public class ServiceValidator : AbstractValidator<Domain.Movidesk.Dtos.Reads.Services.Service>
    {
        public ServiceValidator()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(128)
                .WithMessage("O campo 'name' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.parentServiceId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.parentServiceId));

            RuleFor(x => x.serviceForTicketType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.serviceForTicketType));

            RuleFor(x => x.isVisible)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.isVisible));

            RuleFor(x => x.allowSelection)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.allowSelection));

            RuleFor(x => x.allowFinishTicket)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.allowFinishTicket));

            RuleFor(x => x.isActive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isActive));

            RuleFor(x => x.allowAllCategories)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.allowAllCategories));

            RuleFor(x => x.automationMacro)
                .MaximumLength(64)
                .WithMessage("O campo 'automationMacro' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.automationMacro));

            RuleFor(x => x.defaultCategory)
                .MaximumLength(128)
                .WithMessage("O campo 'defaultCategory' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.defaultCategory));

            RuleFor(x => x.defaultUrgency)
                .MaximumLength(128)
                .WithMessage("O campo 'defaultUrgency' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.defaultUrgency));

            RuleForEach(x => x.categories)
                .MaximumLength(128)
                .WithMessage("Cada categoria deve ter no máximo 128 caracteres.");
        }
    }
}
