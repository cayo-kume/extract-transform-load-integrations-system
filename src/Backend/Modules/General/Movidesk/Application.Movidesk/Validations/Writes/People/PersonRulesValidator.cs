using FluentValidation;

namespace Application.Movidesk.Validations.Writes.People
{
    /// <summary>
    /// regras comuns ao POST e ao PATCH: formato do CPF/CNPJ, um único item principal por lista,
    /// organizações sem repetir e a validação de cada item das listas
    /// </summary>
    public class PersonRulesValidator : AbstractValidator<Domain.Movidesk.Dtos.Writes.People.Person>
    {
        public PersonRulesValidator()
        {
            RuleFor(x => x.cpfCnpj)
                .Matches(@"^\d+$")
                .WithMessage("O campo 'cpfCnpj' deve ter só dígitos.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.cpfCnpj)
                .Length(11)
                .WithMessage("Para pessoa, o 'cpfCnpj' é o CPF, com 11 dígitos.")
                .When(x => x.personType == 1 && !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.cpfCnpj)
                .Length(14)
                .WithMessage("Para empresa, o 'cpfCnpj' é o CNPJ, com 14 dígitos.")
                .When(x => x.personType == 2 && !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.cpfCnpj)
                .Empty()
                .WithMessage("Departamento não tem 'cpfCnpj'.")
                .When(x => x.personType == 4);

            RuleFor(x => x.addresses)
                .Must(x => x!.Count(a => a?.isDefault == true) <= 1)
                .WithMessage("Só um endereço pode ser o principal ('isDefault').")
                .When(x => x.addresses is not null);

            RuleFor(x => x.contacts)
                .Must(x => x!.Count(c => c?.isDefault == true) <= 1)
                .WithMessage("Só um contato pode ser o principal ('isDefault').")
                .When(x => x.contacts is not null);

            RuleFor(x => x.emails)
                .Must(x => x!.Count(e => e?.isDefault == true) <= 1)
                .WithMessage("Só um e-mail pode ser o principal ('isDefault').")
                .When(x => x.emails is not null);

            RuleFor(x => x.relationships)
                .Must(NoRepeatedOrganization)
                .WithMessage("Cada organização só pode aparecer uma vez em 'relationships'.")
                .When(x => x.relationships is not null);

            RuleForEach(x => x.addresses).SetValidator(new AddressValidator());
            RuleForEach(x => x.contacts).SetValidator(new ContactValidator());
            RuleForEach(x => x.emails).SetValidator(new EmailValidator());
            RuleForEach(x => x.relationships).SetValidator(new RelationshipValidator());
            RuleForEach(x => x.customFieldValues).SetValidator(new CustomFieldValueValidator());
        }

        internal static bool NoRepeatedOrganization(List<Domain.Movidesk.Dtos.Writes.People.Relationship>? relationships) =>
            relationships is null ||
            relationships.Where(r => !string.IsNullOrEmpty(r?.id)).GroupBy(r => r!.id).All(g => g.Count() == 1);
    }
}
