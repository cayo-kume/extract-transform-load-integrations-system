using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// valida o corpo da escrita de cliente pessoa física no controller (corpo inválido volta 400): a loja de cadastro, o
    /// cpf com dígito verificador e o nome (exigido sempre: o service não sabe se é inclusão), tamanhos e enums do swagger, no máximo um padrão por
    /// lista. Nulo não muda; "" e 0 zeram, então passam
    /// </summary>
    public class IndividualCustomerValidator : AbstractValidator<IndividualCustomer>
    {
        public IndividualCustomerValidator()
        {
            RuleFor(x => x.cpf)
                .NotEmpty()
                .WithMessage("O campo 'cpf' é obrigatório (é a chave do cadastro).")
                .Must(CustomerRules.IsCpf)
                .WithMessage("O campo 'cpf' deve ter 11 dígitos, sem pontuação, e dígitos verificadores válidos.");

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

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' aceita até 60 caracteres.");

            RuleFor(x => x.rg)
                .MaximumLength(20)
                .WithMessage("O campo 'rg' aceita até 20 caracteres.");

            RuleFor(x => x.rgFederalAgency)
                .MaximumLength(10)
                .WithMessage("O campo 'rgFederalAgency' aceita até 10 caracteres.");

            RuleFor(x => x.birthDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'birthDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.gender)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Gender))
                .WithMessage($"O campo 'gender' só aceita {CustomerRules.Names(CustomerEnumNames.Gender)}.");

            RuleFor(x => x.maritalStatus)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.MaritalStatus))
                .WithMessage($"O campo 'maritalStatus' só aceita {CustomerRules.Names(CustomerEnumNames.MaritalStatus)}.");

            RuleFor(x => x.motherName)
                .MaximumLength(60)
                .WithMessage("O campo 'motherName' aceita até 60 caracteres.");

            RuleFor(x => x.fatherName)
                .MaximumLength(60)
                .WithMessage("O campo 'fatherName' aceita até 60 caracteres.");

            RuleFor(x => x.nationality)
                .MaximumLength(30)
                .WithMessage("O campo 'nationality' aceita até 30 caracteres.");

            RuleFor(x => x.homeTown)
                .MaximumLength(60)
                .WithMessage("O campo 'homeTown' aceita até 60 caracteres.");

            RuleFor(x => x.monthlyIncome)
                .InclusiveBetween(0, 999_999_999_999.99m)
                .WithMessage("O campo 'monthlyIncome' aceita até 12 inteiros (0 zera).")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'monthlyIncome' aceita até 2 casas decimais.")
                .When(x => x.monthlyIncome is not null);

            RuleFor(x => x.occupation)
                .MaximumLength(60)
                .WithMessage("O campo 'occupation' aceita até 60 caracteres.");

            RuleFor(x => x.hireDate)
                .Must(CustomerRules.IsDate)
                .WithMessage("O campo 'hireDate' vai no formato yyyy-MM-dd (ou \"\" para zerar).");

            RuleFor(x => x.workPlace)
                .MaximumLength(60)
                .WithMessage("O campo 'workPlace' aceita até 60 caracteres.");

            RuleFor(x => x.ctps)
                .InclusiveBetween(0, 9_999_999_999)
                .WithMessage("O campo 'ctps' deve ter até 10 dígitos (0 zera).")
                .When(x => x.ctps is not null);

            RuleFor(x => x.ctpsSerial)
                .InclusiveBetween(0, 9_999_999_999)
                .WithMessage("O campo 'ctpsSerial' deve ter até 10 dígitos (0 zera).")
                .When(x => x.ctpsSerial is not null);

            RuleFor(x => x.homePage)
                .MaximumLength(60)
                .WithMessage("O campo 'homePage' aceita até 60 caracteres.");

            RuleFor(x => x.employee)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'employee' deve ter até 9 dígitos (0 zera).")
                .When(x => x.employee is not null);

            RuleFor(x => x.registrationStatus)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.RegistrationStatus))
                .WithMessage($"O campo 'registrationStatus' só aceita {CustomerRules.Names(CustomerEnumNames.RegistrationStatus)}.");

            RuleFor(x => x.suframaCode)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'suframaCode' deve ter até 9 dígitos (0 zera).")
                .When(x => x.suframaCode is not null);

            RuleFor(x => x.taxCodeNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'taxCodeNumber' aceita até 20 caracteres.");

            RuleForEach(x => x.phones)
                .SetValidator(new PhoneInValidator())
                .When(x => x.phones is not null);

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressInValidator())
                .When(x => x.addresses is not null);

            RuleForEach(x => x.emails)
                .SetValidator(new EmailInValidator())
                .When(x => x.emails is not null);

            RuleForEach(x => x.references)
                .SetValidator(new ReferenceInValidator())
                .When(x => x.references is not null);

            RuleForEach(x => x.observations)
                .SetValidator(new ObservationInValidator())
                .When(x => x.observations is not null);

            RuleForEach(x => x.additionalFields)
                .SetValidator(new AdditionalFieldInValidator())
                .When(x => x.additionalFields is not null);

            RuleForEach(x => x.classifications)
                .SetValidator(new ClassificationInValidator())
                .When(x => x.classifications is not null);

            RuleForEach(x => x.limits)
                .SetValidator(new LimitInValidator())
                .When(x => x.limits is not null);

            RuleForEach(x => x.socialNetworks)
                .SetValidator(new SocialNetworkInValidator())
                .When(x => x.socialNetworks is not null);

            RuleFor(x => x.individualAdditionals!)
                .SetValidator(new IndividualAdditionalValidator())
                .When(x => x.individualAdditionals is not null);

            RuleForEach(x => x.bankAccounts)
                .SetValidator(new BankAccountInValidator())
                .When(x => x.bankAccounts is not null);
        }
    }
}
