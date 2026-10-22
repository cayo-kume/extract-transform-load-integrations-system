using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// corpo do POST /persons/relationships: ao menos um relacionamento, sem repetir organização
    /// </summary>
    public class RelationshipsValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Relationships>
    {
        public RelationshipsValidator()
        {
            RuleFor(x => x.relationships)
                .NotEmpty()
                .WithMessage("Informe ao menos um relacionamento em 'relationships'.");

            RuleFor(x => x.relationships)
                .Must(PersonRulesValidator.NoRepeatedOrganization)
                .WithMessage("Cada organização só pode aparecer uma vez em 'relationships'.");

            RuleForEach(x => x.relationships)
                .SetValidator(new RelationshipValidator());
        }
    }
}
