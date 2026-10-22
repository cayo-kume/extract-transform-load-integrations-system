using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Services
{
    /// <summary>
    /// criação de serviço (POST): barra a chamada antes da API, porque 3 erros seguidos bloqueiam a API por 60 s
    /// </summary>
    public class PostServiceValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Services.Service>
    {
        public PostServiceValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("O campo 'name' é obrigatório.");

            RuleFor(x => x.serviceForTicketType)
                .Must(x => x is 0 or 1 or 2)
                .WithMessage("O campo 'serviceForTicketType' é obrigatório: 0 (público), 1 (interno) ou 2 (públicos e internos).");

            RuleFor(x => x.isVisible)
                .Must(x => x is 1 or 2 or 3)
                .WithMessage("O campo 'isVisible' é obrigatório: 1 (agente), 2 (cliente) ou 3 (agente e cliente).");

            RuleFor(x => x.allowSelection)
                .Must(x => x is 1 or 2 or 3)
                .WithMessage("O campo 'allowSelection' é obrigatório: 1 (agente), 2 (cliente) ou 3 (agente e cliente).");

            RuleFor(x => x.allowFinishTicket)
                .NotNull()
                .WithMessage("O campo 'allowFinishTicket' é obrigatório.");

            RuleFor(x => x.isActive)
                .NotNull()
                .WithMessage("O campo 'isActive' é obrigatório.");

            RuleFor(x => x.allowAllCategories)
                .NotNull()
                .WithMessage("O campo 'allowAllCategories' é obrigatório.");

            RuleFor(x => x.categories)
                .NotEmpty()
                .WithMessage("Com 'allowAllCategories' falso, informe ao menos uma categoria em 'categories'.")
                .When(x => x.allowAllCategories == false);

            Include(new ServiceRulesValidator());
        }
    }
}
