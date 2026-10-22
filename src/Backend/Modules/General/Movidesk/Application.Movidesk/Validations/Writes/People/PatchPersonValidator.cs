using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// alteração de pessoa (PATCH): ao menos um campo e sem id no corpo (o id vai na rota).
    /// Listas enviadas substituem a lista inteira na Movidesk (quem chama manda a lista completa).
    /// </summary>
    public class PatchPersonValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Person>
    {
        public PatchPersonValidator()
        {
            RuleFor(x => x)
                .Must(HasAnyField)
                .WithMessage("Informe ao menos um campo para alterar.");

            RuleFor(x => x.id)
                .Null()
                .WithMessage("O 'id' (Cód. Ref.) não pode ser alterado: ele vai só na rota.");

            RuleFor(x => x.personType)
                .Must(x => x is 1 or 2 or 4)
                .WithMessage("O campo 'personType' deve ser 1 (pessoa), 2 (empresa) ou 4 (departamento).")
                .When(x => x.personType is not null);

            RuleFor(x => x.profileType)
                .Must(x => x is 1 or 2 or 3)
                .WithMessage("O campo 'profileType' deve ser 1 (agente), 2 (cliente) ou 3 (agente e cliente).")
                .When(x => x.profileType is not null);

            Include(new PersonRulesValidator());
        }

        private static bool HasAnyField(Domain.Movidesk.Dtos.Writes.People.Person x) =>
            typeof(Domain.Movidesk.Dtos.Writes.People.Person).GetProperties().Any(p => p.GetValue(x) is not null);
    }
}
