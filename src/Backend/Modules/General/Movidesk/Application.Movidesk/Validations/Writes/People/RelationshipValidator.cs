using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// relacionamento: o indicador de contrato forçado é obrigatório, e sem acesso a todos os serviços é preciso listar ao menos um
    /// </summary>
    public class RelationshipValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Relationship>
    {
        public RelationshipValidator()
        {
            RuleFor(x => x.forceChildrenToHaveSomeAgreement)
                .NotNull()
                .WithMessage("O campo 'forceChildrenToHaveSomeAgreement' do relacionamento é obrigatório.");

            RuleFor(x => x.services)
                .NotEmpty()
                .WithMessage("Com 'allowAllServices' falso, informe ao menos um serviço em 'services'.")
                .When(x => x.allowAllServices == false);

            RuleForEach(x => x.services)
                .Must(x => x?.id > 0)
                .WithMessage("Todo serviço do relacionamento precisa do 'id'.");
        }
    }
}
