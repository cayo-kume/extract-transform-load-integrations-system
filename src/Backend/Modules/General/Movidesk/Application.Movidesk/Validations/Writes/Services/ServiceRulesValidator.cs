using FluentValidation;

namespace Application.Movidesk.Validations.Writes.Services
{
    /// <summary>
    /// regras comuns ao POST e ao PATCH: tamanhos do layout, categorias sem repetir e categoria padrão dentro da lista
    /// </summary>
    public class ServiceRulesValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.Services.Service>
    {
        public ServiceRulesValidator()
        {
            RuleFor(x => x.name)
                .MaximumLength(128)
                .WithMessage("O campo 'name' deve ter no máximo 128 caracteres.");

            RuleFor(x => x.automationMacro)
                .MaximumLength(64)
                .WithMessage("O campo 'automationMacro' deve ter no máximo 64 caracteres.");

            RuleFor(x => x.defaultCategory)
                .MaximumLength(128)
                .WithMessage("O campo 'defaultCategory' deve ter no máximo 128 caracteres.");

            RuleFor(x => x.defaultUrgency)
                .MaximumLength(128)
                .WithMessage("O campo 'defaultUrgency' deve ter no máximo 128 caracteres.");

            RuleFor(x => x.parentServiceId)
                .GreaterThan(0)
                .WithMessage("O campo 'parentServiceId' deve ser o id de um serviço.")
                .When(x => x.parentServiceId is not null);

            RuleForEach(x => x.categories)
                .NotEmpty()
                .WithMessage("As categorias não podem ser vazias.")
                .MaximumLength(128)
                .WithMessage("Cada categoria deve ter no máximo 128 caracteres.");

            RuleFor(x => x.categories)
                .Must(x => x!.Where(c => !string.IsNullOrEmpty(c)).GroupBy(c => c, StringComparer.OrdinalIgnoreCase).All(g => g.Count() == 1))
                .WithMessage("As categorias não podem se repetir.")
                .When(x => x.categories is not null);

            RuleFor(x => x.defaultCategory)
                .Must((x, defaultCategory) => x.categories!.Contains(defaultCategory!, StringComparer.OrdinalIgnoreCase))
                .WithMessage("A 'defaultCategory' precisa estar entre as 'categories'.")
                .When(x => x.allowAllCategories == false && !string.IsNullOrEmpty(x.defaultCategory) && x.categories is not null);
        }
    }
}
