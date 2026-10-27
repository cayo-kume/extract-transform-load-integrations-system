using Application.Totvs.Validations.People.Persons;
using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Representatives;
using FluentValidation;

namespace Application.Totvs.Validations.People.Representatives
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (o representante não tem tamanho de nome e supervisor:
    /// 100, como nas colunas do Script036_Create_Totvs_Representatives_Tables). As filhas comuns usam os validadores da PF
    /// </summary>
    public class RepresentativeValidator : AbstractValidator<Representative>
    {
        public RepresentativeValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));

            RuleFor(x => x.name)
                .MaximumLength(100)
                .WithMessage("O campo 'name' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.supervisorCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.supervisorCode));

            RuleFor(x => x.supervisorName)
                .MaximumLength(100)
                .WithMessage("O campo 'supervisorName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supervisorName));

            RuleFor(x => x.branchInsertCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchInsertCode));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressValidator());

            RuleForEach(x => x.phones)
                .SetValidator(new PhoneValidator());

            RuleForEach(x => x.emails)
                .SetValidator(new EmailValidator());

            RuleForEach(x => x.observations)
                .SetValidator(new ObservationValidator());

            RuleForEach(x => x.additionalFields)
                .SetValidator(new AdditionalFieldValidator());

            RuleForEach(x => x.classifications)
                .SetValidator(new ClassificationValidator());

            RuleForEach(x => x.customers)
                .SetValidator(new RepresentativeCustomerValidator());

            RuleForEach(x => x.priceTables)
                .SetValidator(new RepresentativePriceTableValidator());

            RuleForEach(x => x.salesRegions)
                .SetValidator(new RepresentativeSalesRegionValidator());

            RuleForEach(x => x.representativeParameters)
                .SetValidator(new RepresentativeParameterValidator());
        }
    }
}
