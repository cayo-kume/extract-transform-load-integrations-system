using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Services
{
    /// <summary>
    /// alteração de serviço (PATCH): ao menos um campo, com os valores documentados.
    /// categories enviada substitui a lista inteira na Movidesk (quem chama manda a lista completa).
    /// </summary>
    public class PatchServiceValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Services.Service>
    {
        public PatchServiceValidator()
        {
            RuleFor(x => x)
                .Must(HasAnyField)
                .WithMessage("Informe ao menos um campo para alterar.");

            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("O campo 'name' não pode ficar vazio.")
                .When(x => x.name is not null);

            RuleFor(x => x.serviceForTicketType)
                .Must(x => x is 0 or 1 or 2)
                .WithMessage("O campo 'serviceForTicketType' deve ser 0 (público), 1 (interno) ou 2 (públicos e internos).")
                .When(x => x.serviceForTicketType is not null);

            RuleFor(x => x.isVisible)
                .Must(x => x is 1 or 2 or 3)
                .WithMessage("O campo 'isVisible' deve ser 1 (agente), 2 (cliente) ou 3 (agente e cliente).")
                .When(x => x.isVisible is not null);

            RuleFor(x => x.allowSelection)
                .Must(x => x is 1 or 2 or 3)
                .WithMessage("O campo 'allowSelection' deve ser 1 (agente), 2 (cliente) ou 3 (agente e cliente).")
                .When(x => x.allowSelection is not null);

            RuleFor(x => x.categories)
                .NotEmpty()
                .WithMessage("Ao enviar 'allowAllCategories' falso, informe ao menos uma categoria em 'categories'.")
                .When(x => x.allowAllCategories == false);

            Include(new ServiceRulesValidator());
        }

        private static bool HasAnyField(Domain.Movidesk.Dtos.Writes.Services.Service x) =>
            typeof(Domain.Movidesk.Dtos.Writes.Services.Service).GetProperties().Any(p => p.GetValue(x) is not null);
    }
}
