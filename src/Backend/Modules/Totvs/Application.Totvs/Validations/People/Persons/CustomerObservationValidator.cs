using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class CustomerObservationValidator : AbstractValidator<CustomerObservation>
    {
        public CustomerObservationValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.observation)
                .MaximumLength(80)
                .WithMessage("O campo 'observation' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));

            RuleFor(x => x.isMaintenance)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isMaintenance));

            RuleFor(x => x.LastChangeDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.LastChangeDate));
        }
    }
}
