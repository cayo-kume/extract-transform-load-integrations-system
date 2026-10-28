using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// valida o corpo da escrita de cliente pessoa jurídica no controller (corpo inválido volta 400): a loja de cadastro, o
    /// cnpj com dígito verificador e o nome (exigido sempre: o service não sabe se é inclusão), tamanhos e enums do swagger, no máximo um padrão por
    /// lista. Nulo não muda; "" e 0 zeram, então passam
    /// </summary>
    public class LegalCustomerValidator : AbstractValidator<LegalCustomer>
    {
        public LegalCustomerValidator()
        {
            RuleFor(x => x.cnpj)
                .NotEmpty()
                .WithMessage("O campo 'cnpj' é obrigatório (é a chave do cadastro).")
                .Must(CustomerRules.IsCnpj)
                .WithMessage("O campo 'cnpj' deve ter 14 dígitos, sem pontuação, e dígitos verificadores válidos.");

            RuleFor(x => x.branchInsertCode)
                .NotNull()
                .WithMessage("O campo 'branchInsertCode' é obrigatório (loja de cadastro, da empresa).")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchInsertCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("O campo 'name' é obrigatório (o service não sabe se é inclusão ou alteração).");

            RuleFor(x => x.phones)
                .Must(l => CustomerRules.OneDefault(l!.Select(x => x?.isDefault)))
                .WithMessage("No máximo um telefone com 'isDefault' verdadeiro.")
                .When(x => x.phones is not null);

            RuleFor(x => x.emails)
                .Must(l => CustomerRules.OneDefault(l!.Select(x => x?.isDefault)))
                .WithMessage("No máximo um e-mail com 'isDefault' verdadeiro.")
                .When(x => x.emails is not null);

            RuleFor(x => x.limits)
                .Must(l => l!.Where(x => x?.branchCode is not null).GroupBy(x => x.branchCode).All(g => g.Count() == 1))
                .WithMessage("A mesma loja aparece mais de uma vez em 'limits'.")
                .When(x => x.limits is not null);

            RuleFor(x => x.contacts)
                .Must(l => CustomerRules.OneDefault(l!.Select(x => x?.isDefault)))
                .WithMessage("No máximo um contato com 'isDefault' verdadeiro.")
                .When(x => x.contacts is not null);

            RuleFor(x => x.partners)
                .Must(p => p!.Sum(x => x?.percentageParticipation ?? 0) <= 100)
                .WithMessage("A participação dos sócios soma mais de 100%.")
                .When(x => x.partners is not null);

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' aceita até 60 caracteres.");

            RuleFor(x => x.fantasyName)
                .MaximumLength(60)
                .WithMessage("O campo 'fantasyName' aceita até 60 caracteres.");

            RuleFor(x => x.uf)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.State))
                .WithMessage($"O campo 'uf' só aceita {CustomerRules.Names(CustomerEnumNames.State)}.");

            RuleFor(x => x.numberStateRegistration)
                .MaximumLength(18)
                .WithMessage("O campo 'numberStateRegistration' aceita até 18 caracteres.");

            RuleFor(x => x.dateFoundation)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'dateFoundation' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.codeActivity)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'codeActivity' deve ter até 2 dígitos (0 zera).")
                .When(x => x.codeActivity is not null);

            RuleFor(x => x.numberEmployees)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.NumberEmployer))
                .WithMessage($"O campo 'numberEmployees' só aceita {CustomerRules.Names(CustomerEnumNames.NumberEmployer)}.");

            RuleFor(x => x.monthlyInvoicing)
                .InclusiveBetween(0, 999_999_999_999.99m)
                .WithMessage("O campo 'monthlyInvoicing' aceita até 12 inteiros (0 zera).")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'monthlyInvoicing' aceita até 2 casas decimais.")
                .When(x => x.monthlyInvoicing is not null);

            RuleFor(x => x.shareCapital)
                .InclusiveBetween(0, 999_999_999_999.99m)
                .WithMessage("O campo 'shareCapital' aceita até 12 inteiros (0 zera).")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'shareCapital' aceita até 2 casas decimais.")
                .When(x => x.shareCapital is not null);

            RuleFor(x => x.homePage)
                .MaximumLength(100)
                .WithMessage("O campo 'homePage' aceita até 100 caracteres.");

            RuleFor(x => x.registrationStatus)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.RegistrationStatus))
                .WithMessage($"O campo 'registrationStatus' só aceita {CustomerRules.Names(CustomerEnumNames.RegistrationStatus)}.");

            RuleFor(x => x.codeActivityCnae)
                .InclusiveBetween(0, 9_999_999)
                .WithMessage("O campo 'codeActivityCnae' deve ter até 7 dígitos (0 zera).")
                .When(x => x.codeActivityCnae is not null);

            RuleFor(x => x.codeActivityCnae2)
                .InclusiveBetween(0, 9_999_999)
                .WithMessage("O campo 'codeActivityCnae2' deve ter até 7 dígitos (0 zera).")
                .When(x => x.codeActivityCnae2 is not null);

            RuleFor(x => x.typeTaxRegime)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.TaxRegime))
                .WithMessage($"O campo 'typeTaxRegime' só aceita {CustomerRules.Names(CustomerEnumNames.TaxRegime)}.");

            RuleFor(x => x.typeSubTributary)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.SubTributary))
                .WithMessage($"O campo 'typeSubTributary' só aceita {CustomerRules.Names(CustomerEnumNames.SubTributary)}.");

            RuleFor(x => x.suframaCode)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'suframaCode' deve ter até 9 dígitos (0 zera).")
                .When(x => x.suframaCode is not null);

            RuleFor(x => x.registrationMunicipal)
                .MaximumLength(18)
                .WithMessage("O campo 'registrationMunicipal' aceita até 18 caracteres.");

            RuleFor(x => x.descriptionJuntaCial)
                .MaximumLength(20)
                .WithMessage("O campo 'descriptionJuntaCial' aceita até 20 caracteres.");

            RuleFor(x => x.dateRegJuntaCial)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'dateRegJuntaCial' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.foreignIdentification)
                .MaximumLength(20)
                .WithMessage("O campo 'foreignIdentification' aceita até 20 caracteres.");

            RuleFor(x => x.initialDateActivity)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'initialDateActivity' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.finalDateActivity)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'finalDateActivity' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleForEach(x => x.phones)
                .SetValidator(new PhoneInValidator())
                .When(x => x.phones is not null);

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressInValidator())
                .When(x => x.addresses is not null);

            RuleForEach(x => x.emails)
                .SetValidator(new EmailInValidator())
                .When(x => x.emails is not null);

            RuleForEach(x => x.partners)
                .SetValidator(new PartnerInValidator())
                .When(x => x.partners is not null);

            RuleForEach(x => x.observations)
                .SetValidator(new ObservationInValidator())
                .When(x => x.observations is not null);

            RuleForEach(x => x.additionalFields)
                .SetValidator(new AdditionalFieldInValidator())
                .When(x => x.additionalFields is not null);

            RuleForEach(x => x.classifications)
                .SetValidator(new ClassificationInValidator())
                .When(x => x.classifications is not null);

            RuleForEach(x => x.references)
                .SetValidator(new ReferenceInValidator())
                .When(x => x.references is not null);

            RuleForEach(x => x.socialNetworks)
                .SetValidator(new SocialNetworkInValidator())
                .When(x => x.socialNetworks is not null);

            RuleForEach(x => x.contacts)
                .SetValidator(new ContactInValidator())
                .When(x => x.contacts is not null);

            RuleForEach(x => x.limits)
                .SetValidator(new LimitInValidator())
                .When(x => x.limits is not null);

            RuleFor(x => x.preferences!)
                .SetValidator(new PreferencesValidator())
                .When(x => x.preferences is not null);
        }
    }
}
